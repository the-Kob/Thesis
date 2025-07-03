from sklearn.feature_selection import VarianceThreshold

from data_processing.process_data import get_merged_dataframe
from sklearn import svm
from sklearn.model_selection import train_test_split, GridSearchCV
from sklearn.metrics import classification_report
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.decomposition import PCA


def run_model():
    df = get_merged_dataframe()
    df = df.dropna(subset=["label"])  # remove data lacking a label
    df["label"] = df["label"].astype("category").cat.codes

    X = df.drop(columns=["study_id", "player", "label"])
    y = df["label"]

    # After trying to use feature selection ('select', SelectKBest(score_func=f_classif, k=10)),
    # it discovered that the feature in the 9th index did not vary (same_distance_trend_count).
    # When this was confirmed, the decision to drop that feature was made as it was irrelevant
    selector = VarianceThreshold(threshold=0.0)
    X_reduced = selector.fit_transform(X)

    print("Splitting data...")
    X_train, X_test, y_train, y_test = train_test_split(
        X_reduced, y, test_size=0.2, stratify=y, random_state=42
    )

    pipeline = Pipeline([
        ('scaler', StandardScaler()),
        ('svm', svm.SVC(kernel="linear", C=1, gamma='scale', class_weight='balanced'))
    ])

    pipeline.fit(X_train, y_train)
    y_pred = pipeline.predict(X_train)
    print("Classification Report (Baseline):")
    print(classification_report(y_train, y_pred))

    # With PCA
    pipeline_with_pca = Pipeline([
        ('scaler', StandardScaler()),
        ('pca', PCA()),
        ('svm', svm.SVC(class_weight='balanced'))
    ])
    param_grid_with_pca = {
        'svm__C': [0.1, 1, 10],
        'svm__gamma': [0.01, 0.1, 1],
        'svm__kernel': ['rbf', 'linear'],
        'pca__n_components': [5, 8, 10, 12, 15],
    }
    grid_with_pca = GridSearchCV(pipeline_with_pca, param_grid_with_pca, cv=3, scoring='f1_macro')
    grid_with_pca.fit(X_train, y_train)

    # Without PCA
    pipeline_no_pca = Pipeline([
        ('scaler', StandardScaler()),
        ('svm', svm.SVC(class_weight='balanced'))
    ])
    param_grid_no_pca = {
        'svm__C': [0.1, 1, 10],
        'svm__gamma': [0.01, 0.1, 1],
        'svm__kernel': ['rbf', 'linear'],
    }
    grid_no_pca = GridSearchCV(pipeline_no_pca, param_grid_no_pca, cv=3, scoring='f1_macro')
    grid_no_pca.fit(X_train, y_train)

    print("GridSearchCV best params (with PCA):", grid_with_pca.best_params_)
    print("GridSearchCV best params (no PCA):", grid_no_pca.best_params_)

    # Evaluate with PCA
    grid_params_with_pca = {
        "C": grid_with_pca.best_params_["svm__C"],
        "gamma": grid_with_pca.best_params_.get("svm__gamma", "scale"),
        "kernel": grid_with_pca.best_params_["svm__kernel"],
        "n_components": grid_with_pca.best_params_["pca__n_components"]
    }
    evaluate_final_model(X_train, X_test, y_train, y_test, grid_params_with_pca, "Grid Search WITH PCA")

    # Evaluate without PCA
    grid_params_no_pca = {
        "C": grid_no_pca.best_params_["svm__C"],
        "gamma": grid_no_pca.best_params_.get("svm__gamma", "scale"),
        "kernel": grid_no_pca.best_params_["svm__kernel"],
    }
    evaluate_final_model(X_train, X_test, y_train, y_test, grid_params_no_pca, "Grid Search WITHOUT PCA")


def evaluate_final_model(X_train, X_test, y_train, y_test, best_params, title="Model"):
    steps = [('scaler', StandardScaler())]

    if "n_components" in best_params:
        steps.append(('pca', PCA(n_components=best_params["n_components"])))

    steps.append(('svm', svm.SVC(
        kernel=best_params.get("kernel", "rbf"),
        C=best_params["C"],
        gamma=best_params.get("gamma", "scale"),
        class_weight='balanced'
    )))

    pipeline = Pipeline(steps)

    pipeline.fit(X_train, y_train)
    y_pred = pipeline.predict(X_test)

    print(f"\nClassification Report ({title}):")
    print(classification_report(y_test, y_pred))

if __name__ == "__main__":
    run_model()
