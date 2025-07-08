from sklearn.metrics import classification_report
from sklearn.model_selection import KFold, RepeatedStratifiedKFold, cross_val_score
import pandas as pd

def evaluate_model(pipeline, X_train, X_test, y_train, y_test, title):
    pipeline.fit(X_train, y_train)
    y_pred = pipeline.predict(X_test)

    print(f"\nClassification Report ({title}):")
    print(classification_report(y_test, y_pred))

def evaluate_model_cv(pipeline, X, y, title, n_splits=5, n_repeats=3):
    kf = KFold(n_splits=n_splits, shuffle=True, random_state=42)
    rkf = RepeatedStratifiedKFold(n_splits=n_splits, n_repeats=n_repeats, random_state=42)

    for label, cv in [("K-Fold", kf), ("Repeated K-Fold", rkf)]:
        f1 = cross_val_score(pipeline, X, y, cv=cv, scoring='f1_macro')
        acc = cross_val_score(pipeline, X, y, cv=cv, scoring='accuracy')

        print(f"\n{label} CV Results ({title}):")
        print(f"  Accuracy:  {acc.mean():.3f} ± {acc.std():.3f}")
        print(f"  F1-Score:  {f1.mean():.3f} ± {f1.std():.3f}")