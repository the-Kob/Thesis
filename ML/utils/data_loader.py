from sklearn.ensemble import IsolationForest
from sklearn.feature_selection import VarianceThreshold
from sklearn.model_selection import train_test_split
from data_processing.process_data import get_merged_dataframe

def get_data(remove_outliers=False):
    df = get_merged_dataframe()
    df = df.dropna(subset=["label"])
    df["label"] = df["label"].astype("category").cat.codes

    X = df.drop(columns=["study_id", "player", "label"])
    y = df["label"]

    X = VarianceThreshold(threshold=0.0).fit_transform(X)

    if remove_outliers:
        iso = IsolationForest(contamination=0.05, random_state=42)
        mask = iso.fit_predict(X) != -1
        X, y = X[mask], y[mask]

    return train_test_split(X, y, test_size=0.2, stratify=y, random_state=42)