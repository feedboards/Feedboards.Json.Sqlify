#!/usr/bin/env python3
"""
JSON to ClickHouse Schema Generator

This script analyzes a JSON file and generates a ClickHouse table schema based on the structure.
It can handle nested objects, arrays, and various data types.

Usage:
    python json_to_clickhouse_schema.py <json_file_path> [--table-name <table_name>] [--max-depth <max_depth>]

Example:
    python json_to_clickhouse_schema.py data.json --table-name my_table --max-depth 5
"""

import json
import argparse
import sys
import os
import requests
from collections import defaultdict
from typing import Dict, Any, List, Tuple, Set

# ClickHouse connection details from docker-compose.yml
CLICKHOUSE_HOST = "localhost"
CLICKHOUSE_PORT = 8123
CLICKHOUSE_USER = "default"  # Default username from docker-compose.yml
CLICKHOUSE_PASSWORD = "clickhouse"  # Password from docker-compose.yml
CLICKHOUSE_DATABASE = "default"


def detect_type(value: Any) -> str:
    """Detect the ClickHouse data type for a given JSON value."""
    if value is None:
        return "Nullable(String)"
    elif isinstance(value, bool):
        return "UInt8"
    elif isinstance(value, int):
        if value >= 0:
            if value < 2**8:
                return "UInt8"
            elif value < 2**16:
                return "UInt16"
            elif value < 2**32:
                return "UInt32"
            else:
                return "UInt64"
        else:
            if value >= -2**7 and value < 2**7:
                return "Int8"
            elif value >= -2**15 and value < 2**15:
                return "Int16"
            elif value >= -2**31 and value < 2**31:
                return "Int32"
            else:
                return "Int64"
    elif isinstance(value, float):
        return "Float64"
    elif isinstance(value, str):
        # Check if it's a date or datetime
        if len(value) == 10 and value[4] == '-' and value[7] == '-':
            try:
                year, month, day = map(int, value.split('-'))
                if 1900 <= year <= 2100 and 1 <= month <= 12 and 1 <= day <= 31:
                    return "Date"
            except (ValueError, IndexError):
                pass
        
        if len(value) >= 19 and value[4] == '-' and value[7] == '-' and value[10] == 'T' and value[13] == ':' and value[16] == ':':
            try:
                date_part, time_part = value.split('T')
                year, month, day = map(int, date_part.split('-'))
                if 1900 <= year <= 2100 and 1 <= month <= 12 and 1 <= day <= 31:
                    return "DateTime64(3)"
            except (ValueError, IndexError):
                pass
        
        return "String"
    else:
        return "String"


def analyze_json_structure(json_data: Any, prefix: str = "", max_depth: int = 10, current_depth: int = 0) -> Dict[str, str]:
    """
    Recursively analyze the structure of a JSON object to determine field types.
    Returns a dictionary mapping field paths to their types.
    """
    if current_depth >= max_depth:
        return {}

    structure = {}

    if isinstance(json_data, dict):
        for key, value in json_data.items():
            # Replace special characters in key names
            safe_key = key.replace(' ', '_')  # Only replace spaces, keep dots for path handling
            field_path = f"{prefix}.{safe_key}" if prefix else safe_key

            if isinstance(value, dict):
                # Handle nested object - create a Nested type
                nested_structure = analyze_json_structure(
                    value, "", max_depth, current_depth + 1
                )
                nested_fields = []
                for nested_name, nested_type in sorted(nested_structure.items()):
                    nested_fields.append(f"`{nested_name}` {nested_type}")
                if nested_fields:
                    structure[field_path] = f"Nested(\n        {','.join(nested_fields)}\n    )"
                else:
                    structure[field_path] = "String"  # Default to String if empty object
            elif isinstance(value, list) and value:
                if all(isinstance(item, dict) for item in value):
                    # Create a nested structure for arrays of objects
                    sample = value[0]  # Use the first item as a sample
                    nested_structure = analyze_json_structure(
                        sample, "", max_depth, current_depth + 1
                    )
                    nested_fields = []
                    for nested_name, nested_type in sorted(nested_structure.items()):
                        nested_fields.append(f"`{nested_name}` {nested_type}")
                    if nested_fields:
                        structure[field_path] = f"Nested(\n        {','.join(nested_fields)}\n    )"
                    else:
                        structure[field_path] = "Array(String)"  # Default to Array(String) if empty objects
                elif all(isinstance(item, (str, int, float, bool)) for item in value):
                    # For arrays of primitive types
                    types = [detect_type(item) for item in value if item is not None]
                    if not types:
                        structure[field_path] = "Array(String)"
                    else:
                        # Use the most specific type that can accommodate all values
                        if all(t.startswith("UInt") for t in types):
                            max_type = max(types, key=lambda x: int(x[4:-1]) if x[4:-1].isdigit() else 0)
                            structure[field_path] = f"Array({max_type})"
                        elif all(t.startswith("Int") for t in types):
                            max_type = max(types, key=lambda x: int(x[3:-1]) if x[3:-1].isdigit() else 0)
                            structure[field_path] = f"Array({max_type})"
                        elif any(t == "Float64" for t in types):
                            structure[field_path] = "Array(Float64)"
                        else:
                            structure[field_path] = "Array(String)"
            else:
                structure[field_path] = detect_type(value)

    return structure


def generate_clickhouse_schema(structure: Dict[str, str], table_name: str) -> str:
    """
    Generate a ClickHouse schema from the analyzed structure.
    Handles nested structures and arrays with proper formatting.
    """
    def format_nested_structure(field_type: str, indent_level: int = 2) -> str:
        """Helper function to format nested structures with proper indentation"""
        if not field_type.startswith("Nested("):
            return field_type
        
        # Extract the content between Nested( and )
        content = field_type[7:-1].strip()
        fields = []
        current_field = ""
        nested_level = 0
        
        # Parse the fields while respecting nested structures
        for char in content:
            if char == '(' and 'Nested' in current_field:
                nested_level += 1
            elif char == ')':
                nested_level -= 1
            
            if char == ',' and nested_level == 0:
                fields.append(current_field.strip())
                current_field = ""
            else:
                current_field += char
        
        if current_field:
            fields.append(current_field.strip())
        
        # Format each field
        formatted_fields = []
        base_indent = "    " * indent_level
        for field in fields:
            if "Nested(" in field:
                # Recursively format nested structures
                field_name = field[:field.index("Nested(")].strip()
                nested_content = field[field.index("Nested("):]
                formatted_nested = format_nested_structure(nested_content, indent_level + 1)
                formatted_fields.append(f"{field_name}{formatted_nested}")
            else:
                formatted_fields.append(field)
        
        # Join fields with proper formatting
        fields_str = f",\n{base_indent}".join(formatted_fields)
        return f"Nested(\n{base_indent}{fields_str}\n{base_indent[:-4]})"

    # Generate schema lines
    schema_lines = []

    # Add all fields (both top-level and nested)
    for field_name, field_type in sorted(structure.items()):
        if "." not in field_name:  # Only process top-level fields
            # Format nested structures
            if field_type.startswith("Nested("):
                formatted_type = format_nested_structure(field_type)
                schema_lines.append(f"    `{field_name}` {formatted_type}")
            else:
                schema_lines.append(f"    `{field_name}` {field_type}")

    # Create the final schema
    schema = ",\n".join(schema_lines)
    
    # Generate the complete CREATE TABLE statement
    create_table_sql = f"""
CREATE TABLE IF NOT EXISTS {table_name} (
{schema}
) ENGINE = MergeTree()
ORDER BY tuple();
"""
    
    return create_table_sql


def create_table_in_clickhouse(query: str, table_name: str) -> bool:
    """
    Create a table in ClickHouse with the given schema.
    Uses the credentials from docker-compose.yml.
    """
    clickhouse_url = f"http://{CLICKHOUSE_HOST}:{CLICKHOUSE_PORT}/"

    print(f"ClickHouse URI: {clickhouse_url}")
    
    # Send the query to ClickHouse with authentication
    try:
        response = requests.post(
            clickhouse_url,
            data=query,
            auth=(CLICKHOUSE_USER, CLICKHOUSE_PASSWORD),
            headers={"Content-Type": "application/x-www-form-urlencoded"}
        )
        
        if response.status_code == 200:
            print(f"Table {table_name} created successfully in ClickHouse")
            return True
        else:
            print(f"Error creating table: {response.text}")
            return False
    except Exception as e:
        print(f"Error connecting to ClickHouse: {e}")
        return False


def main():
    parser = argparse.ArgumentParser(description='Generate ClickHouse schema from JSON file')
    parser.add_argument('json_file', help='Path to the JSON file')
    parser.add_argument('--table-name', default='json_table', help='Name for the ClickHouse table')
    parser.add_argument('--max-depth', type=int, default=10, help='Maximum depth to analyze in the JSON structure')
    parser.add_argument('--output', help='Output file for the schema (default: stdout)')
    parser.add_argument('--create-table', action='store_true', help='Create the table in ClickHouse')
    parser.add_argument('--host', default=CLICKHOUSE_HOST, help=f'ClickHouse host (default: {CLICKHOUSE_HOST})')
    parser.add_argument('--port', type=int, default=CLICKHOUSE_PORT, help=f'ClickHouse port (default: {CLICKHOUSE_PORT})')
    parser.add_argument('--user', default=CLICKHOUSE_USER, help=f'ClickHouse user (default: {CLICKHOUSE_USER})')
    parser.add_argument('--password', default=CLICKHOUSE_PASSWORD, help=f'ClickHouse password (default: {CLICKHOUSE_PASSWORD})')
    parser.add_argument('--database', default=CLICKHOUSE_DATABASE, help=f'ClickHouse database (default: {CLICKHOUSE_DATABASE})')
    
    args = parser.parse_args()
    
    try:
        # Convert relative path to absolute path if needed
        json_file_path = os.path.abspath(args.json_file)
        
        # Check if file exists
        if not os.path.exists(json_file_path):
            print(f"Error: File not found: {json_file_path}", file=sys.stderr)
            return 1
            
        # Read the JSON file
        print(f"Reading JSON file: {json_file_path}", file=sys.stderr)
        with open(json_file_path, 'r', encoding='utf-8') as f:
            try:
                json_data = json.load(f)
            except json.JSONDecodeError as e:
                print(f"Error parsing JSON: {e}", file=sys.stderr)
                return 1
        
        # Analyze the structure
        print("Analyzing JSON structure...", file=sys.stderr)
        structure = analyze_json_structure(json_data, max_depth=args.max_depth)
        
        # Generate the schema
        print("Generating ClickHouse schema...", file=sys.stderr)
        schema = generate_clickhouse_schema(structure, args.table_name)

        print(" ")
        print(schema)
        print(" ")
        
        # Output the schema
        if args.output:
            output_path = os.path.abspath(args.output)
            print(f"Writing schema to: {output_path}", file=sys.stderr)
            with open(output_path, 'w', encoding='utf-8') as f:
                f.write(schema)
            print(f"Schema written to {output_path}", file=sys.stderr)
        else:
            print(schema)
        
        # Create the table in ClickHouse if requested
        if args.create_table:
            print(f"Creating table {args.table_name} in ClickHouse...", file=sys.stderr)
            create_table_in_clickhouse(schema, args.table_name)
        
        return 0
    
    except Exception as e:
        print(f"Error: {e}", file=sys.stderr)
        import traceback
        traceback.print_exc(file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main()) 