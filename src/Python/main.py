import json
from clickhouse_connect import get_client

# ClickHouse connection details
CLICKHOUSE_HOST = "localhost"
CLICKHOUSE_PORT = 8123
CLICKHOUSE_USER = "default"
CLICKHOUSE_PASSWORD = "clickhouse"
CLICKHOUSE_DATABASE = "default"
CLICKHOUSE_TABLE = "simple"

# Connect to ClickHouse
client = get_client(
    host=CLICKHOUSE_HOST,
    port=CLICKHOUSE_PORT,
    username=CLICKHOUSE_USER,
    password=CLICKHOUSE_PASSWORD,
    database=CLICKHOUSE_DATABASE
)

# Use a raw string for Windows file path
# json_file_path = r'F:\Projects\src\Newtonsoft.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\SimpleTest\test.json'
json_file_path = r'F:\Projects\src\Newtonsoft.Json.Sqlify\test\Feedboards.Json.Sqlify.CLI\test\SimpleTest\simple.json'

# Read JSON data from file
# with open(json_file_path, 'r') as file:
#     data = json.load(file)
#
# # Ensure that JSON data is a list of dictionaries
# if data:
#     # Get the column names from the first dictionary
#     columns = list(data[0].keys())
#     # Convert list of dicts to list of lists with the column order defined by `columns`
#     rows = [[item[col] for col in columns] for item in data]
# else:
#     raise ValueError("JSON data is empty.")
#
# # Insert data into the existing table
# client.insert(table=CLICKHOUSE_TABLE, data=rows, column_names=columns)
#
# print("Data inserted successfully.")

with open(json_file_path, 'r') as file:
    data = json.load(file)

# For Nested type, we need to separate the arrays
if isinstance(data, dict) and 'test' in data:
    # Extract arrays for each nested column
    names = [item['name'] for item in data['test']]
    titles = [item['title'] for item in data['test']]

    # Create the data structure ClickHouse expects for Nested types
    rows = [[names, titles]]  # Single row with arrays
    columns = ['test.name', 'test.title']

    # Insert data into the table
    client.insert(table=CLICKHOUSE_TABLE, data=rows, column_names=columns)
    print("Data inserted successfully.")
else:
    print("No data to insert")