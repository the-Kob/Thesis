from sklearn.ensemble import IsolationForest, RandomForestClassifier
from sklearn.feature_selection import VarianceThreshold, SelectKBest, f_classif
from data_processing.process_data import get_merged_dataframe
from sklearn import svm
from sklearn.model_selection import train_test_split, GridSearchCV, StratifiedKFold
from sklearn.metrics import classification_report
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.decomposition import PCA
from sklearn.model_selection import ParameterGrid

def run_model():
    X_train, X_test, y_train, y_test = get_data(remove_outliers=True)

    evaluate_model(build_pipeline(model_type='svm'),
                   X_train, X_test, y_train, y_test, "Baseline")
    evaluate_model(build_pipeline(use_pca=True, model_type='svm'),
                   X_train, X_test, y_train, y_test, "Baseline + PCA")
    evaluate_model(build_pipeline(use_selectkbest=True, model_type='svm'),
                   X_train, X_test, y_train, y_test, "Baseline + SelectKBest")
    evaluate_model(build_pipeline(use_pca=True, use_selectkbest=True, model_type='svm'),
                   X_train, X_test, y_train, y_test, "Baseline + PCA + SelectKBest")

    evaluate_model(build_pipeline(model_type='rf'),
                   X_train, X_test, y_train, y_test, "Random Forest Baseline")
    evaluate_model(build_pipeline(model_type='rf', use_pca=True),
                   X_train, X_test, y_train, y_test, "Random Forest Baseline + PCA")
    evaluate_model(build_pipeline(model_type='rf', use_selectkbest=True),
                   X_train, X_test, y_train, y_test, "Random Forest Baseline + SelectKBest")
    evaluate_model(build_pipeline(model_type='rf', use_pca=True, use_selectkbest=True),
                   X_train, X_test, y_train, y_test, "Random Forest Baseline + PCA + SelectKBest")

    grid = grid_search(X_train, y_train)
    evaluate_model(grid.best_estimator_,
                   X_train, X_test, y_train, y_test, "Grid Search")

    grid_pca = grid_search(X_train, y_train, use_pca=True)
    evaluate_model(grid_pca.best_estimator_,
                   X_train, X_test, y_train, y_test, "Grid Search + PCA")

    grid_kbest = grid_search(X_train, y_train, use_selectkbest=True)
    evaluate_model(grid_kbest.best_estimator_,
                   X_train, X_test, y_train, y_test, "Grid Search + SelectKBest")

    grid_all = grid_search(X_train, y_train, use_pca=True, use_selectkbest=True)
    evaluate_model(grid_all.best_estimator_,
                   X_train, X_test, y_train, y_test, "Grid Search + PCA + SelectKBest")

def get_data(remove_outliers=False):
    df = get_merged_dataframe()
    df = df.dropna(subset=["label"])
    df["label"] = df["label"].astype("category").cat.codes

    X = df.drop(columns=["study_id", "player", "label"])
    y = df["label"]

    # After trying to use feature selection ('select', SelectKBest(score_func=f_classif, k=10)),
    # it discovered that the feature in the 9th index did not vary (same_distance_trend_count).
    # When this was confirmed, the decision to drop that feature was made as it was irrelevant
    selector = VarianceThreshold(threshold=0.0)
    X = selector.fit_transform(X)

    if remove_outliers:
        iso = IsolationForest(contamination=0.05, random_state=42)
        yhat = iso.fit_predict(X)
        mask = yhat != -1
        X = X[mask]
        y = y[mask]

    return train_test_split(X, y, test_size=0.2, stratify=y, random_state=42)

def build_pipeline(use_pca=False, use_selectkbest=False, model=None, model_type='svm'):
    steps = [('scaler', StandardScaler())]

    if use_selectkbest:
        steps.append(('select', SelectKBest(score_func=f_classif, k=10)))

    if use_pca:
        steps.append(('pca', PCA(n_components=8)))

    if model_type == 'svm':
        steps.append(('svm', model or svm.SVC(kernel="linear", C=1, gamma='scale', class_weight='balanced')))
    else:
        steps.append(('rf', model or RandomForestClassifier(class_weight='balanced', n_estimators=100, random_state=42)))

    return Pipeline(steps)

def grid_search(X_train, y_train, use_pca=False, use_selectkbest=False):
    steps = [('scaler', StandardScaler())]

    if use_selectkbest:
        steps.append(('select', SelectKBest(score_func=f_classif)))

    if use_pca:
        steps.append(('pca', PCA()))

    steps.append(('svm', svm.SVC(class_weight='balanced')))
    pipeline = Pipeline(steps)

    base_grid = {
        'svm__C': [0.1, 1],
        'svm__kernel': ['linear'],
    }

    if use_selectkbest:
        base_grid['select__k'] = [5, 8, 10]

    if use_pca:
        base_grid['pca__n_components'] = [5, 8, 10]

    # If both are used, filter invalid combinations
    if use_pca and use_selectkbest:
        param_grid = [
            {
                'svm__C': [params['svm__C']],
                'svm__kernel': [params['svm__kernel']],
                'pca__n_components': [params['pca__n_components']],
                'select__k': [params['select__k']]
            }
            for params in ParameterGrid(base_grid)
            if params['pca__n_components'] <= params['select__k']
        ]
    else:
        # This handles the other cases safely
        param_grid = [
            {key: [value] for key, value in params.items()}
            for params in ParameterGrid(base_grid)
        ]

    grid = GridSearchCV(
        pipeline,
        param_grid,
        cv=StratifiedKFold(n_splits=3, shuffle=True, random_state=42),
        scoring='f1_macro',
    )

    grid.fit(X_train, y_train)

    return grid

def evaluate_model(pipeline, X_train, X_test, y_train, y_test, title):
    pipeline.fit(X_train, y_train)
    y_pred = pipeline.predict(X_test)
    print(f"\nClassification Report ({title}):")
    print(classification_report(y_test, y_pred))

if __name__ == "__main__":
    # Run your usual experiments
    run_model()

