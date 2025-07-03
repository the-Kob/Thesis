import pandas as pd
import tkinter as tk
from tkinter import filedialog
import os

def select_existing_master_file():
    print("Select existing master file (or cancel if none)...")
    root = tk.Tk()
    root.withdraw()

    file_path = filedialog.askopenfilename(
        title="Select Existing Master CSV (optional)",
        filetypes=[("CSV files", "*.csv"), ("All files", "*.*")]
    )

    return file_path if file_path else None

def select_files_to_merge():
    print("Select one or more new gameplay data files to merge...")
    root = tk.Tk()
    root.withdraw()

    files = filedialog.askopenfilenames(
        title="Select New Gameplay CSV Files",
        filetypes=[("CSV files", "*.csv"), ("All files", "*.*")]
    )

    return list(files)

def select_output_folder():
    print("Select destination folder to save the final merged file...")
    root = tk.Tk()
    root.withdraw()

    folder_path = filedialog.askdirectory(
        title="Select Folder to Save Merged CSV"
    )

    return folder_path

def merge_csv_files(existing_file, new_files):
    dfs = []

    if existing_file:
        print(f"Loading existing master file: {existing_file}")
        master_df = pd.read_csv(existing_file)
        dfs.append(master_df)

    for file in new_files:
        print(f"Loading: {file}")
        df = pd.read_csv(file)
        dfs.append(df)

    merged_df = pd.concat(dfs, ignore_index=True).drop_duplicates()
    print(f"Merged {len(dfs)} files. Final shape: {merged_df.shape}")

    return merged_df

def run_merge():
    existing_file = select_existing_master_file()
    new_files = select_files_to_merge()

    if not new_files:
        print("No new files selected. Exiting.")
        return

    output_folder = select_output_folder()
    if not output_folder:
        print("No output folder selected. Exiting.")
        return

    merged_df = merge_csv_files(existing_file, new_files)

    default_filename = "merged_gameplay_data.csv"
    output_path = os.path.join(output_folder, default_filename)

    merged_df.to_csv(output_path, index=False)
    print(f"\n✅ Merged file saved to:\n{output_path}")

if __name__ == "__main__":
    run_merge()
