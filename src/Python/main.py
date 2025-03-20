import json
from clickhouse_connect import get_client

# ClickHouse connection details
CLICKHOUSE_HOST = "localhost"
CLICKHOUSE_PORT = 8123
CLICKHOUSE_USER = "default"
CLICKHOUSE_PASSWORD = "clickhouse"
CLICKHOUSE_DATABASE = "default"
CLICKHOUSE_TABLE = "adyawatercom_products_1"

# Connect to ClickHouse
client = get_client(
    host=CLICKHOUSE_HOST,
    port=CLICKHOUSE_PORT,
    username=CLICKHOUSE_USER,
    password=CLICKHOUSE_PASSWORD,
    database=CLICKHOUSE_DATABASE
)

# Use a raw string for Windows file path
json_file_path = r'F:/Projects/src/Feedboards.Json.Sqlify/test/Feedboards.Json.Sqlify.CLI/test/adyawater.com_products_1.json'

# Read JSON data from file
with open(json_file_path, 'r') as file:
    data = json.load(file)

# Extract products array
if isinstance(data, dict) and 'products' in data:
    products = data['products']
    
    # Prepare data for insertion
    rows = []
    
    for product in products:
        # Create a single row with all nested data
        row = {
            'products.id': [product.get('id')],
            'products.title': [product.get('title')],
            'products.handle': [product.get('handle')],
            'products.body_html': [product.get('body_html')],
            'products.published_at': [product.get('published_at')],
            'products.created_at': [product.get('created_at')],
            'products.updated_at': [product.get('updated_at')],
            'products.vendor': [product.get('vendor')],
            'products.product_type': [product.get('product_type')],
            'products.tags': product.get('tags', []),  # Already an array
            
            # Handle variants
            'products.variants.id': [],
            'products.variants.product_id': [],
            'products.variants.title': [],
            'products.variants.option1': [],
            'products.variants.option2': [],
            'products.variants.option3': [],
            'products.variants.sku': [],
            'products.variants.requires_shipping': [],
            'products.variants.taxable': [],
            'products.variants.available': [],
            'products.variants.price': [],
            'products.variants.grams': [],
            'products.variants.compare_at_price': [],
            'products.variants.position': [],
            'products.variants.created_at': [],
            'products.variants.updated_at': [],
            'products.variants.featured_image': [],
            
            # Handle images
            'products.images.id': [],
            'products.images.product_id': [],
            'products.images.position': [],
            'products.images.created_at': [],
            'products.images.updated_at': [],
            'products.images.src': [],
            'products.images.width': [],
            'products.images.height': [],
            'products.images.variant_ids': [],
            
            # Handle options
            'products.options.name': [],
            'products.options.position': [],
            'products.options.values': []
        }
        
        # Add variants data
        if 'variants' in product:
            for variant in product['variants']:
                row['products.variants.id'].append(variant.get('id'))
                row['products.variants.product_id'].append(product.get('id'))
                row['products.variants.title'].append(variant.get('title'))
                row['products.variants.option1'].append(variant.get('option1'))
                row['products.variants.option2'].append(variant.get('option2'))
                row['products.variants.option3'].append(variant.get('option3'))
                row['products.variants.sku'].append(variant.get('sku'))
                row['products.variants.requires_shipping'].append(variant.get('requires_shipping', True))
                row['products.variants.taxable'].append(variant.get('taxable', True))
                row['products.variants.available'].append(variant.get('available', True))
                row['products.variants.price'].append(variant.get('price'))
                row['products.variants.grams'].append(variant.get('grams', 0))
                row['products.variants.compare_at_price'].append(variant.get('compare_at_price'))
                row['products.variants.position'].append(variant.get('position', 1))
                row['products.variants.created_at'].append(variant.get('created_at'))
                row['products.variants.updated_at'].append(variant.get('updated_at'))
                row['products.variants.featured_image'].append(variant.get('featured_image'))
        
        # Add images data
        if 'images' in product:
            for image in product['images']:
                row['products.images.id'].append(image.get('id'))
                row['products.images.product_id'].append(product.get('id'))
                row['products.images.position'].append(image.get('position'))
                row['products.images.created_at'].append(image.get('created_at'))
                row['products.images.updated_at'].append(image.get('updated_at'))
                row['products.images.src'].append(image.get('src'))
                row['products.images.width'].append(image.get('width'))
                row['products.images.height'].append(image.get('height'))
                row['products.images.variant_ids'].append(image.get('variant_ids', []))
        
        # Add options data
        if 'options' in product:
            for option in product['options']:
                row['products.options.name'].append(option.get('name'))
                row['products.options.position'].append(option.get('position'))
                row['products.options.values'].append(option.get('values', []))
        
        rows.append(row)
    
    # Insert all data
    if rows:
        client.insert(table=CLICKHOUSE_TABLE, data=rows)
        print(f"Successfully inserted {len(rows)} products with their nested data.")
else:
    print("No products data found in the JSON file.")