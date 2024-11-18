import json
import pandas as pd
import sys
import os
from openpyxl.utils import get_column_letter


def main():

    input_json = '/Users/boyudu/Desktop/csci526/TeamProject/data.json'  # 输入的 JSON 文件路径
    output_csv = '/Users/boyudu/Desktop/csci526/TeamProject/long.csv'  # 输出的 CSV 文件路径
    output_excel = '/Users/boyudu/Desktop/csci526/TeamProject/long.xlsx'  # 输出的 Excel 文件路径（可选）
    total_waves = 5  #total wave

    # check input exist
    if not os.path.exists(input_json):
        print(f"错误：文件 '{input_json}' 未找到。请确保文件存在并路径正确。")
        sys.exit(1)

    # read JSON
    try:
        with open(input_json, 'r', encoding='utf-8') as f:
            data = json.load(f)
    except json.JSONDecodeError as je:
        print(f"错误：解析 JSON 文件时发生错误：{je}")
        sys.exit(1)
    except Exception as e:
        print(f"读取 JSON 文件时发生未知错误：{e}")
        sys.exit(1)

    # check JSON structure
    if "gameResults" not in data:
        print("错误：JSON 文件中缺少 'gameResults' 键。")
        sys.exit(1)

    game_results = data["gameResults"]

    # store list
    extracted_data = []

    # read game results
    for record_id, record_data in game_results.items():
        charge_times = record_data.get("chargeTimesPerWave", [])

        # if charge_times is not list or null，set [0] * total_waves
        if not isinstance(charge_times, list) or not charge_times:
            charge_times = [0] * total_waves
        else:
            # replace charge_times's  None with 0
            charge_times = [ct if isinstance(ct, (int, float)) else 0 for ct in charge_times]
            # check list length is total_waves
            if len(charge_times) < total_waves:
                charge_times += [0] * (total_waves - len(charge_times))
            elif len(charge_times) > total_waves:
                charge_times = charge_times[:total_waves]

        #  charge_times convert to long table
        for idx, charge_time in enumerate(charge_times, start=1):
            wave_label = f"Wave {idx}"
            extracted_data.append({
                "Wave": wave_label,
                "Charge Time": charge_time
            })

    # check data
    if not extracted_data:
        print("错误：未从 JSON 文件中提取到任何 'chargeTimesPerWave' 数据。")
        sys.exit(1)

    # create DataFrame
    df = pd.DataFrame(extracted_data)

    # print preview
    print("提取的数据预览:")
    print(df.head())

    # save as CSV
    try:
        df.to_csv(output_csv, index=False, encoding='utf-8-sig')
        print(f"充电时间数据已成功保存到 '{output_csv}' 文件中。")
    except Exception as e:
        print(f"写入 CSV 文件时发生错误：{e}")
        sys.exit(1)

    # save as xlxs
    try:
        with pd.ExcelWriter(output_excel, engine='openpyxl') as writer:
            df.to_excel(writer, sheet_name='Charge Times', index=False)
            worksheet = writer.sheets['Charge Times']

            for idx, col in enumerate(df.columns, 1):
                max_length = max(df[col].astype(str).map(len).max(), len(col)) + 2
                column_letter = get_column_letter(idx)
                worksheet.column_dimensions[column_letter].width = max_length

        print(f"充电时间数据已成功保存到 '{output_excel}' 文件中。")
    except Exception as e:
        print(f"写入 Excel 文件时发生错误：{e}")
        sys.exit(1)


if __name__ == "__main__":
    main()
