## All Data
| Model                                               | Accuracy (K-Fold) | F1-Score (K-Fold) |    Accuracy (RKF) |    F1-Score (RKF) |
|-----------------------------------------------------|------------------:|------------------:|------------------:|------------------:|
| Logistic Regression Baseline                        |     0.640 ± 0.088 |     0.638 ± 0.087 |     0.636 ± 0.091 |     0.633 ± 0.092 |
| Logistic Regression + PCA                           |     0.620 ± 0.065 |     0.619 ± 0.066 |     0.569 ± 0.070 |     0.565 ± 0.069 |
| Logistic Regression + SelectKBest                   |     0.620 ± 0.054 |     0.620 ± 0.054 |     0.593 ± 0.091 |     0.591 ± 0.092 |
| Logistic Regression + PCA + SelectKBest             |     0.640 ± 0.071 |     0.639 ± 0.071 |     0.589 ± 0.094 |     0.587 ± 0.094 |
| Logistic Regression Grid Search                     |     0.640 ± 0.088 |     0.638 ± 0.087 |     0.636 ± 0.091 |     0.633 ± 0.092 |
| Logistic Regression Grid Search + PCA               |     0.627 ± 0.057 |     0.625 ± 0.056 |     0.576 ± 0.068 |     0.573 ± 0.069 |
| Logistic Regression Grid Search + SelectKBest       |     0.620 ± 0.054 |     0.620 ± 0.054 |     0.593 ± 0.091 |     0.591 ± 0.092 |
| Logistic Regression Grid Search + PCA + SelectKBest |     0.640 ± 0.071 |     0.639 ± 0.071 |     0.589 ± 0.094 |     0.587 ± 0.094 |
| SVM Baseline                                        |     0.613 ± 0.105 |     0.612 ± 0.105 |     0.609 ± 0.100 |     0.606 ± 0.101 |
| SVM + PCA                                           |     0.593 ± 0.065 |     0.592 ± 0.066 |     0.573 ± 0.067 |     0.569 ± 0.070 |
| SVM + SelectKBest                                   |     0.620 ± 0.058 |     0.619 ± 0.058 |     0.591 ± 0.105 |     0.590 ± 0.105 |
| SVM + PCA + SelectKBest                             |     0.633 ± 0.070 |     0.633 ± 0.069 |     0.591 ± 0.102 |     0.588 ± 0.104 |
| SVM Grid Search                                     |     0.633 ± 0.087 |     0.632 ± 0.087 |     0.622 ± 0.099 |     0.619 ± 0.099 |
| SVM Grid Search + PCA                               |     0.647 ± 0.069 |     0.645 ± 0.068 |     0.591 ± 0.086 |     0.587 ± 0.087 |
| SVM Grid Search + SelectKBest                       |     0.620 ± 0.058 |     0.619 ± 0.058 |     0.591 ± 0.105 |     0.590 ± 0.105 |
| SVM Grid Search + PCA + SelectKBest                 |     0.620 ± 0.058 |     0.619 ± 0.058 |     0.591 ± 0.105 |     0.590 ± 0.105 |
| Random Forest Baseline                              |     0.640 ± 0.057 |     0.639 ± 0.057 |     0.562 ± 0.082 |     0.559 ± 0.083 |
| Random Forest + PCA                                 |     0.580 ± 0.034 |     0.578 ± 0.036 |     0.547 ± 0.088 |     0.543 ± 0.089 |
| Random Forest + SelectKBest                         |     0.573 ± 0.083 |     0.566 ± 0.083 |     0.558 ± 0.071 |     0.555 ± 0.072 |
| Random Forest + PCA + SelectKBest                   |     0.627 ± 0.090 |     0.624 ± 0.091 |     0.578 ± 0.087 |     0.575 ± 0.087 |
| LightGBM Baseline                                   |     0.640 ± 0.068 |     0.639 ± 0.068 |     0.593 ± 0.086 |     0.591 ± 0.086 |
| LightGBM + PCA                                      |     0.613 ± 0.040 |     0.613 ± 0.040 |     0.567 ± 0.096 |     0.564 ± 0.097 |
| LightGBM + SelectKBest                              |     0.607 ± 0.065 |     0.602 ± 0.065 |     0.549 ± 0.068 |     0.545 ± 0.069 |
| LightGBM + PCA + SelectKBest                        |     0.540 ± 0.049 |     0.535 ± 0.048 |     0.553 ± 0.088 |     0.549 ± 0.089 |
| LightGBM Grid Search                                |     0.640 ± 0.068 |     0.639 ± 0.068 |     0.593 ± 0.086 |     0.591 ± 0.086 |
| LightGBM Grid Search + PCA                          |     0.613 ± 0.040 |     0.613 ± 0.040 |     0.567 ± 0.096 |     0.564 ± 0.097 |
| LightGBM Grid Search + SelectKBest                  |     0.587 ± 0.086 |     0.583 ± 0.086 |     0.556 ± 0.075 |     0.553 ± 0.076 |
| LightGBM Grid Search + PCA + SelectKBest            | **0.653 ± 0.040** | **0.650 ± 0.042** |     0.547 ± 0.100 |     0.544 ± 0.101 |

## Best Model Variants
| Model Family            | Best Variant (RKF)                         |    Accuracy (RKF) |    F1-Score (RKF) |
|------------------------|--------------------------------------------|------------------:|------------------:|
| Logistic Regression     | Grid Search                                | **0.636 ± 0.091** | **0.633 ± 0.092** |
| SVM                     | Grid Search                                |     0.622 ± 0.099 |     0.619 ± 0.099 |
| Random Forest           | PCA + SelectKBest                          |     0.578 ± 0.087 |     0.575 ± 0.087 |
| LightGBM                | Baseline                                   |     0.593 ± 0.086 |     0.591 ± 0.086 |


**K-Fold** cross validation was initially used to explore different model configurations, and although it yielded slightly better scores, this was likely due to lower variation across the specific folds.
For more reliable model comparison, we relied on **Repeated K-Fold (RKF)** cross validation, which reduces variance by averaging over multiple randomized splits.

The best-performing model was the **Logistic Regression** with hyperparameters tuned via **Grid Search**, achieving an *Accuracy* of 0.636 ± 0.091 and a *F1-Score* of 0.633 ± 0.092.

Although **PCA** and **SelectKBest** are commonly used for dimensionality reduction and feature relevance, applying them in this context consistently led to lower *Accuracy* and *F1-Score*. Given the small dataset size (≈188 samples total), these transformations may have removed useful signal or introduced unnecessary complexity, ultimately hurting generalization.