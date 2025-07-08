from utils.data_loader import get_data
from utils.pipeline_builder import build_pipeline
from utils.grid_search import grid_search
from utils.evaluator import evaluate_model, evaluate_model_cv
import warnings
warnings.filterwarnings("ignore", message="X does not have valid feature names")


def run_models(remove_outliers=False, use_cv=False):
    X_train, X_test, y_train, y_test = get_data(remove_outliers)

    configs = [
        ("Logistic Regression Baseline", 'logreg', False, False),
        ("Logistic Regression + PCA", 'logreg', True, False),
        ("Logistic Regression + SelectKBest", 'logreg', False, True),
        ("Logistic Regression + PCA + SelectKBest", 'logreg', True, True),

        ("Random Forest Baseline", 'rf', False, False),
        ("Random Forest + PCA", 'rf', True, False),
        ("Random Forest + SelectKBest", 'rf', False, True),
        ("Random Forest + PCA + SelectKBest", 'rf', True, True),

        ("LightGBM Baseline", 'lgbm', False, False),
        ("LightGBM + PCA", 'lgbm', True, False),
        ("LightGBM + SelectKBest", 'lgbm', False, True),
        ("LightGBM + PCA + SelectKBest", 'lgbm', True, True),

        ("SVM Baseline", 'svm', False, False),
        ("SVM + PCA", 'svm', True, False),
        ("SVM + SelectKBest", 'svm', False, True),
        ("SVM + PCA + SelectKBest", 'svm', True, True),
    ]

    for title, model_type, use_pca, use_selectkbest in configs:
        pipeline = build_pipeline(model_type=model_type, use_pca=use_pca, use_selectkbest=use_selectkbest)
        if use_cv:
            evaluate_model_cv(pipeline, X_train, y_train, title)
        else:
            evaluate_model(pipeline, X_train, X_test, y_train, y_test, title)

    grid_configs = [
        ("Grid Search Baseline", False, False),
        ("Grid Search + PCA", True, False),
        ("Grid Search + SelectKBest", False, True),
        ("Grid Search + PCA + SelectKBest", True, True)
    ]

    for title, use_pca, use_selectkbest in grid_configs:
        # Logistic Regression Grid Search
        grid = grid_search(X_train, y_train, use_pca, use_selectkbest, model_type='logreg')
        model_title = f"Logistic Regression {title}"
        if use_cv:
            evaluate_model_cv(grid.best_estimator_, X_train, y_train, model_title)
        else:
            evaluate_model(grid.best_estimator_, X_train, X_test, y_train, y_test, model_title)

        # SVM Grid Search
        grid = grid_search(X_train, y_train, use_pca, use_selectkbest, model_type='svm')
        model_title = f"SVM {title}"
        if use_cv:
            evaluate_model_cv(grid.best_estimator_, X_train, y_train, model_title)
        else:
            evaluate_model(grid.best_estimator_, X_train, X_test, y_train, y_test, model_title)

        # LightGBM Grid Search
        grid = grid_search(X_train, y_train, use_pca, use_selectkbest, model_type='lgbm')
        model_title = f"LightGBM {title}"
        if use_cv:
            evaluate_model_cv(grid.best_estimator_, X_train, y_train, model_title)
        else:
            evaluate_model(grid.best_estimator_, X_train, X_test, y_train, y_test, model_title)


if __name__ == "__main__":
    run_models()