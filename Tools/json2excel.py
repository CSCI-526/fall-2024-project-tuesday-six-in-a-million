import json
import pandas as pd


# total waves
TOTAL_WAVES = 5

# Read JSON
with open('data.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

# Store List
records = []


for record_id, record_data in data["gameResults"].items():
    main_record = {
        "id": record_id,
        "finalWave": record_data.get("finalWave"),
        "flashlightUsageCount": record_data.get("flashlightUsageCount"),
        "result": record_data.get("result"),
        "timestamp": record_data.get("timestamp"),
        "totalGameTime": record_data.get("totalGameTime"),
    }

    # read Tower Data，exclude chargingFrequency
    if "towerData" in record_data:
        for i, tower in enumerate(record_data["towerData"], 1):
            main_record[f"tower_{i}_totalKillCount"] = tower.get("totalKillCount")
            main_record[f"tower_{i}_totalChargeTime"] = tower.get("totalChargeTime")
    else:
        # if do not have tower data, set None
        main_record["tower_1_totalKillCount"] = None
        main_record["tower_1_totalChargeTime"] = None

    # read flashlightDurations
    if "flashlightDurations" in record_data:
        for idx, duration in enumerate(record_data["flashlightDurations"], 1):
            main_record[f"duration_{idx}"] = duration
    else:
        main_record["duration_1"] = None

    if "chargeTimesPerWave" in record_data:
        for idx, charge_time in enumerate(record_data["chargeTimesPerWave"], 1):
            main_record[f"chargeTimePerWave_{idx}"] = charge_time
        # if length less than TOTAL_WAVES，set None
        for idx in range(len(record_data["chargeTimesPerWave"]) + 1, TOTAL_WAVES + 1):
            main_record[f"chargeTimePerWave_{idx}"] = None
    else:
        # if don't have chargeTimesPerWave，fill all chargeTimePerWave with None
        for idx in range(1, TOTAL_WAVES + 1):
            main_record[f"chargeTimePerWave_{idx}"] = None
    # add main_record to records
    records.append(main_record)

# convert to DataFrame
df = pd.DataFrame(records)

# get all col name
all_columns = list(df.columns)

# set duration after totalGameTime
# get totalGameTime index
total_game_time_index = all_columns.index('totalGameTime')

# divide duration col
duration_columns = [col for col in df.columns if col.startswith('duration_')]
duration_columns.sort(key=lambda x: int(x.split('_')[1]))  # 按编号排序

# rank order
new_columns_order = (
    all_columns[:total_game_time_index + 1] +
    duration_columns +
    [col for col in all_columns[total_game_time_index + 1:] if col not in duration_columns]
)

# rank DataFrame
df = df[new_columns_order]

# save as Excel
df.to_excel('output.xlsx', index=False)
