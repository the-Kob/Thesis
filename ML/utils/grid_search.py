from lightgbm import LGBMClassifier
from sklearn.linear_model import LogisticRegression
from sklearn.pipeline import Pipeline
from sklearn.model_selection import GridSearchCV, StratifiedKFold, ParameterGrid
from sklearn.preprocessing import StandardScaler
from sklearn.decomposition import PCA
from sklearn.feature_selection import SelectKBest, f_classif
from sklearn import svm

def grid_search(X_train, y_train, use_pca=False, use_selectkbest=False, model_type = svm):
    steps = [('scaler', StandardScaler())]

    if use_selectkbest:
        steps.append(('select', SelectKBest(score_func=f_classif)))
    if use_pca:
        steps.append(('pca', PCA()))

    if model_type == 'svm': # use SVM classifier
        steps.append(('clf', svm.SVC(probability=True, class_weight='balanced')))
        base_grid = {
            'clf__C': [0.01, 0.1, 1],
            'clf__kernel': ['linear'],
        }
    elif model_type == 'lgbm': # use LightGBM classifier
        steps.append(('clf', LGBMClassifier(verbose=-1, class_weight='balanced')))
        base_grid = {
            'clf__n_estimators': [50, 100],
            'clf__learning_rate': [0.01, 0.1],
            'clf__num_leaves': [15, 31],
        }
    elif model_type == 'logreg':
        steps.append(('clf', LogisticRegression(max_iter=1000, class_weight='balanced', random_state=42)))
        base_grid = {
            'clf__C': [0.01, 0.1, 1, 10],
            'clf__penalty': ['l2'],
            'clf__solver': ['lbfgs'],
        }
    else:
        raise ValueError("Unsupported model_type for grid search")

    pipeline = Pipeline(steps)

    if use_selectkbest:
        base_grid['select__k'] = [5, 8, 10]
    if use_pca:
        base_grid['pca__n_components'] = [5, 8, 10]

    if use_pca and use_selectkbest:
        param_grid = [
            {k: [v] for k, v in params.items()}
            for params in ParameterGrid(base_grid)
            if params['pca__n_components'] <= params['select__k']
        ]
    else:
        param_grid = [
            {k: [v] for k, v in params.items()}
            for params in ParameterGrid(base_grid)
        ]

    grid = GridSearchCV(pipeline, param_grid, cv=StratifiedKFold(n_splits=3, shuffle=True, random_state=42))
    grid.fit(X_train, y_train)
    return grid