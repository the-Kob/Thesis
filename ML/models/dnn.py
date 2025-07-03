from data_processing.process_data import get_merged_dataframe
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from sklearn.preprocessing import StandardScaler
import tensorflow as tf
from tensorflow.keras import layers, models
import pandas as pd

def run_model():
    # Load data
    df = get_merged_dataframe()

    # Remove data with no label
    df = df.dropna(subset=["label"])

    # Encode labels
    df["label"] = df["label"].astype("category").cat.codes
    num_classes = df["label"].nunique()

    X = df.drop(columns=["study_id", "player", "label"])
    y = df["label"]

    # Feature scaling
    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X)

    # Train/test split
    X_train, X_test, y_train, y_test = train_test_split(X_scaled, y, test_size=0.2, stratify=y, random_state=42)

    # Convert labels to categorical if multi-class
    y_train_cat = tf.keras.utils.to_categorical(y_train, num_classes)
    y_test_cat = tf.keras.utils.to_categorical(y_test, num_classes)

    print("Building and training DNN...")
    model = models.Sequential([
        layers.Input(shape=(X.shape[1],)),
        layers.Dense(64, activation="relu"),
        layers.Dense(64, activation="relu"),
        layers.Dense(num_classes, activation="softmax" if num_classes > 2 else "sigmoid")
    ])

    model.compile(
        optimizer="adam",
        loss="categorical_crossentropy" if num_classes > 2 else "binary_crossentropy",
        metrics=["accuracy"]
    )

    model.fit(X_train, y_train_cat, epochs=30, batch_size=32, validation_split=0.1, verbose=1)

    print("Evaluating model...")
    y_pred_probs = model.predict(X_test)
    y_pred = y_pred_probs.argmax(axis=1)
    print(classification_report(y_test, y_pred))

if __name__ == "__main__":
    run_model()
