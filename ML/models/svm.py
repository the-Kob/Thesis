from data_processing.process_data import get_merged_dataframe
from sklearn import svm
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report

def run_model():
    # Load data
    df = get_merged_dataframe()

    # Remove data with no label
    df = df.dropna(subset=["label"])

    # Simple preprocessing: encode labels and choose features
    df["label"] = df["label"].astype("category").cat.codes

    X = df.drop(columns=["study_id", "player", "label"])
    y = df["label"]

    print("Splitting data...")
    # stratify = y makes the split proportional to the class distribution in y
    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, stratify=y, random_state=42)

    print("Training SVM...")
    clf = svm.SVC(kernel="linear")
    clf.fit(X_train, y_train)

    print("Evaluating model...")
    y_pred = clf.predict(X_test)
    print(classification_report(y_test, y_pred))

if __name__ == "__main__":
    run_model()
