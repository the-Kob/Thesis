import pandas as pd
from collections import defaultdict
import json
import os

# Set paths based on your directory structure
data_dir = r"D:\\Unity Projects\\Thesis\\Assets\\StreamingAssets\\Results\\Events Data"
script_dir = r"D:\\Unity Projects\\Thesis\\Assets\\StreamingAssets\\ML\\Events Data Processing"
input_file = os.path.join(data_dir, "Events.csv")
output_file = os.path.join(script_dir, "summary_output.csv")

# Define known actuators and receivers
KNOWN_ACTUATORS = ["P1", "P2"]
KNOWN_RECEIVERS = ["P1Enemy", "P2Enemy"]

def load_data(filepath):
    return pd.read_csv(filepath)

def summarize_events(df):
    print(df)

    # Garante que study_id é string limpa
    df["study_id"] = df["study_id"].astype(str).str.strip()

    grouped = defaultdict(dict)
    flat_summary = []

    for study_id, study_df in df.groupby("study_id"):
        summary = {"study_id": study_id}

        # Count of each event_type
        event_type_counts = study_df["event_type"].value_counts().to_dict()
        summary.update({f"event_type_{k}": v for k, v in event_type_counts.items()})

        # Count of known actuators only
        for actuator in KNOWN_ACTUATORS:
            summary[f"actuator_{actuator}"] = (study_df["event_actuator"] == actuator).sum()

        # Count of known receivers only
        for receiver in KNOWN_RECEIVERS:
            summary[f"receiver_{receiver}"] = (study_df["event_receiver"] == receiver).sum()

        # Distance and time statistics
        summary["avg_distance"] = study_df["distance_between_players"].mean()
        summary["total_time"] = study_df["elapse_time"].max()

        # Trend frequencies
        distance_trends = study_df["distance_trend"].value_counts().to_dict()
        movement_trends = study_df["movement_trend"].value_counts().to_dict()
        summary.update({f"distance_trend_{k}": v for k, v in distance_trends.items()})
        summary.update({f"movement_trend_{k}": v for k, v in movement_trends.items()})

        grouped[study_id] = summary
        flat_summary.append(summary)

    return grouped, pd.DataFrame(flat_summary)


def export_summary_to_csv(df, output_path):
    df.to_csv(output_path, index=False)
    print(f"Summary exported to {output_path}")

def print_summary(summary_dict):
    for study_id, data in summary_dict.items():
        print(f"\nStudy ID: {study_id}")
        for key, val in data.items():
            if key != "study_id":
                print(f"{key}: {val}")

if __name__ == "__main__":
    df = load_data(input_file)
    summary_dict, summary_df = summarize_events(df)
    export_summary_to_csv(summary_df, output_file)
