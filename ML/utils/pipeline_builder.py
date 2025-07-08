from sklearn.linear_model import LogisticRegression
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.decomposition import PCA
from sklearn.feature_selection import SelectKBest, f_classif
from sklearn import svm
from sklearn.ensemble import RandomForestClassifier
from lightgbm import LGBMClassifier

def build_pipeline(use_pca=False, use_selectkbest=False, model=None, model_type='svm'):
    steps = [('scaler', StandardScaler())]

    if use_selectkbest:
        steps.append(('select', SelectKBest(score_func=f_classif, k=10)))
    if use_pca:
        steps.append(('pca', PCA(n_components=8)))

    if model_type == 'svm':
        steps.append(('svm', model or svm.SVC(kernel="linear", C=1, gamma='scale', probability=True, class_weight='balanced')))
    elif model_type == 'rf':
        steps.append(('rf', model or RandomForestClassifier(n_estimators=100, random_state=42)))
    elif model_type == 'lgbm':
        steps.append(('lgbm', model or LGBMClassifier(class_weight='balanced', random_state=42, verbose=-1)))
    elif model_type == 'logreg':
        steps.append(('logreg', model or LogisticRegression(max_iter=1000, class_weight='balanced', random_state=42)))

    return Pipeline(steps)