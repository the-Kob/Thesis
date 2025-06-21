import pandas as pd
from collections import defaultdict
import json
import os
import csv
import tkinter as tk
from tkinter import filedialog

PLAYERS = ["P1", "P2"]

event_player_map = {
    "Buff": "event_actuator",
    "Nerf": "event_actuator",
    "SecondaryAttack": "event_actuator",
    "GettingHit": "event_receiver",
    "EnemyHit": "event_actuator",
    "EnemyKill": "event_actuator",
    "BulletMiss": "event_actuator",
    "Displacement": "event_actuator"
}

def select_files():
    root = tk.Tk()
    root.withdraw()

    script_dir = os.path.dirname(os.path.abspath(__file__))

    thesis_dir = os.path.abspath(os.path.join(script_dir, '..', '..'))

    gd_file_path = filedialog.askopenfilename(
        title="Select the Gameplay Data",
        initialdir=thesis_dir,
        filetypes=[("CSV files", "*.csv"), ("All files", "*.*")]
    )

    fd_file_path = filedialog.askopenfilename(
        title="Select the Form Data",
        initialdir=thesis_dir,
        filetypes=[("CSV files", "*.csv"), ("All files", "*.*")]
    )

    return gd_file_path, fd_file_path

def load_data(filepath):
    return pd.read_csv(filepath)

def get_player(row):
    field = event_player_map.get(row["event_type"])
    if pd.isna(field):
        return None
    return row[field] if pd.notna(row[field]) else None

def process_events(data):
    print("Processing gameplay events...")

    data["study_id"] = data["study_id"].astype(str).str.strip()

    data["player"] = data.apply(get_player, axis=1)
    data = data.dropna(subset=["player"])

    # 'study_id', 'score', 'combo', 'event_type', 'event_actuator',
    # 'event_receiver', 'elapse_time', 'distance_between_players',
    # 'distance_since_last_displacement_trigger', 'distance_trend',
    # 'movement_trend', 'is_first_playthrough'

    flat_summary = []

    for (study_id, player), study_data in data.groupby(["study_id", "player"]):
        summary = {"study_id": study_id, "player": player}

        # General Metrics
        summary["avg_score"] = study_data["score"].mean()
        summary["max_score"] = study_data["score"].max()

        summary["avg_combo"] = study_data["combo"].mean()
        summary["max_combo"] = study_data["combo"].max()

        summary["avg_distance_between_players"] = study_data["distance_between_players"].mean()
        summary["max_distance_between_players"] = study_data["distance_between_players"].max()

        # Displacement Metrics
        displacement_data = study_data[study_data["event_type"] == "Displacement"]

        summary["avg_distance_since_last_displacement"] = displacement_data["distance_since_last_displacement_trigger"].mean()
        summary["max_distance_since_last_displacement"] = displacement_data["distance_since_last_displacement_trigger"].max()

        closer_count = (displacement_data["distance_trend"] == "Closer").sum()
        summary["closer_distance_trend_count"] = closer_count
        same_count = (displacement_data["distance_trend"] == "Same").sum()
        summary["same_distance_trend_count"] = same_count
        farther_count = (displacement_data["distance_trend"] == "Farther").sum()
        summary["farther_distance_trend_count"] = farther_count

        toward_count = (displacement_data["movement_trend"] == "Toward").sum()
        summary["toward_movement_trend_count"] = toward_count
        away_count = (displacement_data["movement_trend"] == "Away").sum()
        summary["away_movement_trend_count"] = away_count
        sideways_count = (displacement_data["movement_trend"] == "Sideways").sum()
        summary["sideways_movement_trend_count"] = sideways_count

        # Buff & Nerf Metrics
        summary["buff_count"] = (study_data["event_type"] == "Buff").sum()
        summary["buff_on_p1enemy_count"] = ((study_data["event_type"] == "Buff") & (study_data["event_receiver"] == "P1Enemy")).sum()
        summary["buff_on_p2enemy_count"] = ((study_data["event_type"] == "Buff") & (study_data["event_receiver"] == "P2Enemy")).sum()

        summary["nerf_count"] = (study_data["event_type"] == "Nerf").sum()
        summary["nerf_on_p1enemy_count"] = ((study_data["event_type"] == "Nerf") & (study_data["event_receiver"] == "P1Enemy")).sum()
        summary["nerf_on_p2enemy_count"] = ((study_data["event_type"] == "Nerf") & (study_data["event_receiver"] == "P2Enemy")).sum()

        summary["effect_on_p1enemy_count"] = summary["buff_on_p1enemy_count"] + summary["buff_on_p1enemy_count"]
        summary["effect_on_p2enemy_count"] = summary["nerf_on_p1enemy_count"] + summary["nerf_on_p1enemy_count"]

        # Other Metrics
        summary["secondary_attack_count"] = (study_data["event_type"] == "SecondaryAttack").sum()
        summary["getting_hit_count"] = (study_data["event_type"] == "GettingHit").sum()
        summary["bullet_miss_count"] = (study_data["event_type"] == "BulletMiss").sum()
        summary["enemy_kill_count"] = (study_data["event_type"] == "EnemyKill").sum()
        summary["enemy_hit_count"] = (study_data["event_type"] == "EnemyHit").sum()

        flat_summary.append(summary)

    return pd.DataFrame(flat_summary)

def process_form(data):
    print("Processing form...")

    data["player"] = data["Which player are you?"].str.extract(r"\((P\d)\)")

    label_mappings = []

    for i in range(1, 5):  # Adjust the range if there are more than 9 study IDs
        study_id_col = f"Study ID {i}"
        label_col = f"{i}. How will you play (indicated by the researcher)?"

        if study_id_col in data.columns and label_col in data.columns:
            subset = data[[study_id_col, "player", label_col]].dropna()

            for _, row in subset.iterrows():
                study_id = str(row[study_id_col]).strip()
                player = row["player"]
                label = row[label_col]
                label_mappings.append({
                    "study_id": study_id,
                    "player": player,
                    "label": label
                })

    return pd.DataFrame(label_mappings)


def get_merged_dataframe():
    gameplay_data_file_path, form_data_file_path = select_files()
    gd = load_data(gameplay_data_file_path)
    fd = load_data(form_data_file_path)

    events_df = process_events(gd)
    label_df = process_form(fd)

    final_df = pd.merge(events_df, label_df, how="left", on=["study_id", "player"])

    # Construct path to data_processing directory
    current_dir = os.path.dirname(os.path.abspath(__file__))
    data_processing_dir = os.path.join(current_dir, '..', 'data_processing')
    os.makedirs(data_processing_dir, exist_ok=True)  # Ensure directory exists

    # Save to CSV
    temp_csv_path = os.path.join(data_processing_dir, 'last_merged_data.csv')
    final_df.to_csv(temp_csv_path, index=False)

    print(f"Saved merged data to {temp_csv_path}")

    return final_df
