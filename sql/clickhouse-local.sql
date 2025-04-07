CREATE TABLE in_network_data (
    reporting_entity_name String,
    reporting_entity_type String,
    plan_name String,
    plan_id String,
    plan_id_type String,
    last_updated_on Date,
    in_network Nested (
        negotiation_arrangement String,
        name String,
        billing_code String,
        billing_code_type String,
        billing_code_type_version String,
        description String,
        negotiated_rates Nested (
            provider_groups Array(String),
            negotiated_prices Nested (
                billing_class String,
                negotiated_rate Float64,
                expiration_date Date,
                negotiated_type String,
                service_code Array(String),
                billing_code_modifier Array(String)
            )
        )
    )
) ENGINE = MergeTree()
ORDER BY (reporting_entity_name, plan_id, last_updated_on);


CREATE TABLE IF NOT EXISTS table_2 (
    `reporting_entity_name` String,
    `reporting_entity_type` String
) ENGINE = MergeTree()
ORDER BY tuple();


SELECT * FROM file('/var/lib/clickhouse/user_files/json/*.json', 'JSONEachRow');

CREATE TABLE IF NOT EXISTS shopy_test_1 (
    `products` Nested(
                `body_html` String,
                `created_at` DateTime64(3),`handle` String,`id` UInt64,`images`Nested(
                    `created_at` DateTime64(3
                ),
                `height` UInt16,
                `id` UInt64,
                `position` UInt8,
                `product_id` UInt64,
                `src` String,
                `updated_at` DateTime64(3),`variant_ids` String,`width` UInt16
            ),
            `options`Nested(
                    `name` String,
                    `position` UInt8,
                    `values` Array(String)
            ),
            `product_type` String,
            `published_at` DateTime64(3),
            `tags` String,
            `title` String,
            `updated_at` DateTime64(3),
            `variants`Nested(
                        `available` UInt8,
                        `compare_at_price` String,
                        `created_at` DateTime64(3),
                        `featured_image` Nullable(String),
                        `grams` UInt8,
                        `id` UInt64,
                        `option1` String,
                        `option2` Nullable(String),
                        `option3` Nullable(String),
                        `position` UInt8,
                        `price` String,
                        `product_id` UInt64,
                        `requires_shipping` UInt8,
                        `sku` String,
                        `taxable` UInt8,
                        `title` String,
                        `updated_at` DateTime64(3)
            ),
            `vendor` Stri
            )
        )
    )
) ENGINE = MergeTree()
ORDER BY tuple();



CREATE TABLE IF NOT EXISTS shopy_test_1 (
    `products` Nested(
                `body_html` String,
                `created_at` DateTime64(3),`handle` String,`id` UInt64,`images`Nested(
                    `created_at` DateTime64(3
                ),
                `height` UInt16,
                `id` UInt64,
                `position` UInt8,
                `product_id` UInt64,
                `src` String,
                `updated_at` DateTime64(3),`variant_ids` String,`width` UInt16
            ),
            `options`Nested(
                    `name` String,
                    `position` UInt8,
                    `values` Array(String)
            ),
            `product_type` String,
            `published_at` DateTime64(3),
            `tags` String,
            `title` String,
            `updated_at` DateTime64(3),
            `variants`Nested(
                        `available` UInt8,
                        `compare_at_price` String,
                        `created_at` DateTime64(3),
                        `featured_image` Nullable(String),
                        `grams` UInt8,
                        `id` UInt64,
                        `option1` String,
                        `option2` Nullable(String),
                        `option3` Nullable(String),
                        `position` UInt8,
                        `price` String,
                        `product_id` UInt64,
                        `requires_shipping` UInt8,
                        `sku` String,
                        `taxable` UInt8,
                        `title` String,
                        `updated_at` DateTime64(3)
            ),
            `vendor` String
            )
        ) ENGINE = MergeTree()
ORDER BY tuple();

CREATE TABLE IF NOT EXISTS test_1 (
    `products` Nested(
        `body_html` String,
        `created_at` DateTime64(3),
        `handle` String,
        `id` UInt64,
        `images` Nested(
            `created_at` DateTime64(3),
            `height` UInt16,
            `id` UInt64,
            `position` UInt8,
            `product_id` UInt64,
            `src` String,
            `updated_at` DateTime64(3),
            `variant_ids` String,
            `width` UInt16
        ),
        `options` Nested(
            `name` String,
            `position` UInt8,
            `values` Array(String)
        ),
        `product_type` String,
        `published_at` DateTime64(3),
        `tags` String,
        `title` String,
        `updated_at` DateTime64(3),
        `variants` Nested(
            `available` UInt8,
            `compare_at_price` Nullable(String),
            `created_at` DateTime64(3),
            `featured_image` Nullable(String),
            `grams` UInt16,
            `id` UInt64,
            `option1` String,
            `option2` Nullable(String),
            `option3` Nullable(String),
            `position` UInt8,
            `price` String,
            `product_id` UInt64,
            `requires_shipping` UInt8,
            `sku` Nullable(String),
            `taxable` UInt8,
            `title` String,
            `updated_at` DateTime64(3)
        ),
        `vendor` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();


SELECT * FROM file('/var/lib/clickhouse/user_files/json/*.json', 'JSONEachRow');

INSERT INTO test_2
SELECT *
FROM file('/var/lib/clickhouse/user_files/json/*.json', 'JSONEachRow');


SELECT *
FROM default.adyawatercom_products_1
LIMIT 501;


CREATE TABLE IF NOT EXISTS simple (
    `test` Nested(
        `name` String,
        `title` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();


CREATE TABLE IF NOT EXISTS test_2 (
    `products` Nested(
        `body_html` String,
        `created_at` DateTime64(3),
        `handle` String,
        `id` UInt64,
        `images` Nested(
            `created_at` DateTime64(3),
            `height` UInt64,
            `id` UInt64,
            `position` UInt64,
            `product_id` UInt64,
            `src` String,
            `updated_at` DateTime64(3),
            `variant_ids` Array(String),
            `width` UInt64
        ),
        `options` Nested(
            `name` String,
            `position` UInt64,
            `values` Array(String)
        ),
        `product_type` String,
        `published_at` DateTime64(3),
        `tags` Array(String),
        `title` String,
        `updated_at` DateTime64(3),
        `variants` Nested(
            `available` UInt8,
            `compare_at_price` Nullable(String),
            `created_at` DateTime64(3),
            `featured_image` Nullable(String),
            `grams` UInt64,
            `id` UInt64,
            `option1` String,
            `option2` Nullable(String),
            `option3` Nullable(String),
            `position` UInt64,
            `price` String,
            `product_id` UInt64,
            `requires_shipping` UInt8,
            `sku` Nullable(String),
            `taxable` UInt8,
            `title` String,
            `updated_at` DateTime64(3)
        ),
        `vendor` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();

CREATE TABLE my_first_table
(
    user_id UInt32,
    message String,
    timestamp DateTime,
    metric Float32
)
ENGINE = MergeTree()
PRIMARY KEY (user_id, timestamp);

CREATE TABLE IF NOT EXISTS test (
    `message` String,
    `metric` Float64,
    `timestamp` DateTime64(3),
    `user_id` UInt64
) ENGINE = MergeTree()
ORDER BY tuple();


INSERT INTO test (user_id, message, timestamp, metric) VALUES
    (101, 'Hello, ClickHouse!',                                 now(),       -1.0    ),
    (102, 'Insert a lot of rows per batch',                     yesterday(), 1.41421 ),
    (102, 'Sort your data based on your commonly-used queries', today(),     2.718   ),
    (101, 'Granules are the smallest chunks of data read',      now() + 5,   3.14159 );


SELECT * FROM my_first_table;

SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS table_2 (
    `products` Nested(
        `body_html` String,
        `created_at` DateTime64(3),
        `handle` String,
        `id` UInt64,
        `images` Nested(
            `created_at` DateTime64(3),
            `height` UInt64,
            `id` UInt64,
            `position` UInt64,
            `product_id` UInt64,
            `src` String,
            `updated_at` DateTime64(3),
            `variant_ids` Array(String),
            `width` UInt64
        ),
        `options` Nested(
            `name` String,
            `position` UInt64,
            `values` Array(String)
        ),
        `product_type` String,
        `published_at` DateTime64(3),
        `tags` Array(String),
        `title` String,
        `updated_at` DateTime64(3),
        `variants` Nested(
            `available` UInt8,
            `compare_at_price` Nullable(String),
            `created_at` DateTime64(3),
            `featured_image` Nullable(String),
            `grams` UInt64,
            `id` UInt64,
            `option1` String,
            `option2` Nullable(String),
            `option3` Nullable(String),
            `position` UInt64,
            `price` String,
            `product_id` UInt64,
            `requires_shipping` UInt8,
            `sku` Nullable(String),
            `taxable` UInt8,
            `title` String,
            `updated_at` DateTime64(3)
        ),
        `vendor` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();

SHOW CREATE TABLE table_2;

INSERT INTO test VALUES (
    [  -- Start of products array
        (   -- Start of product tuple
            -- Basic product fields
            '<h2 data-mce-fragment="1">Enhance your Potable Water</h2>',  -- body_html
            '2024-03-05 17:56:52.000',                                    -- created_at
            'water-bucket-filter-wholesale-without-adya-clarity',          -- handle
            8227960193212,                                                -- id

            -- images nested
            [   -- Start of images array
                (   -- Start of image tuple
                    '2024-03-05 17:57:31.000',  -- created_at
                    1024,                        -- height
                    36973422313660,              -- id
                    1,                           -- position
                    8227960193212,              -- product_id
                    'https://cdn.shopify.com/s/files/1/0287/1046/files/waterbucketfiltersystem_4.png?v=1709679453',  -- src
                    '2024-03-05 17:57:33.000',  -- updated_at
                    [],                          -- variant_ids array
                    1024                         -- width
                )
            ],

            -- options nested
            [   -- Start of options array
                (   -- Start of option tuple
                    'Title',           -- name
                    1,                 -- position
                    ['Default Title']  -- values array
                )
            ],

            'Drinking Water Filter System',        -- product_type
            '2024-03-05 17:58:11.000',            -- published_at
            [],                                    -- tags array
            'Water Bucket Filter Wholesale (Without Adya Clarity)',  -- title
            '2024-07-15 23:05:53.000',            -- updated_at

            -- variants nested
            [   -- Start of variants array
                (   -- Start of variant tuple
                    1,                              -- available
                    NULL,                           -- compare_at_price
                    '2024-03-05 17:56:52.000',     -- created_at
                    NULL,                           -- featured_image
                    2268,                           -- grams
                    43414518956220,                 -- id
                    'Default Title',                -- option1
                    NULL,                           -- option2
                    NULL,                           -- option3
                    1,                              -- position
                    '64.99',                        -- price
                    8227960193212,                  -- product_id
                    1,                              -- requires_shipping
                    NULL,                           -- sku
                    1,                              -- taxable
                    'Default Title',                -- title
                    '2024-07-15 23:05:53.000'      -- updated_at
                )
            ],

            'Adya, Inc.'  -- vendor
        )
    ]
);

select * from table_2;

SELECT
    products.id,
    products.title,
    products.handle,
    products.product_type,
    products.vendor
FROM table_2;


SELECT
    products.id,
    products.title,
    products.variants.id as variant_id,
    products.variants.price,
    products.variants.title as variant_title
FROM table_2
ARRAY JOIN products.variants;

SELECT
    products.id,
    products.title,
    products.images.src as image_url,
    products.images.width,
    products.images.height
FROM table_2
ARRAY JOIN products.images;

SELECT
    products.id,
    products.title,
    products.options.name as option_name,
    products.options.values as option_values
FROM table_2
ARRAY JOIN products.options;

INSERT INTO test
SELECT *
FROM file(
    '/var/lib/clickhouse/user_files/*.json',
    'JSONEachRow'
);

SELECT
    products.created_at
FROM test;

SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS test (
    `products` Nested(
        `body_html` String,
        `created_at` DateTime64(3),
        `handle` String,
        `id` UInt64,
        `images` Nested(
            `created_at` DateTime64(3),
            `height` UInt64,
            `id` UInt64,
            `position` UInt64,
            `product_id` UInt64,
            `src` String,
            `updated_at` DateTime64(3),
            `variant_ids` Array(String),
            `width` UInt64
        ),
        `options` Nested(
            `name` String,
            `position` UInt64,
            `values` Array(String)
        ),
        `product_type` String,
        `published_at` DateTime64(3),
        `tags` Array(String),
        `title` String,
        `updated_at` DateTime64(3),
        `variants` Nested(
            `available` UInt8,
            `compare_at_price` Nullable(String),
            `created_at` DateTime64(3),
            `featured_image` Nullable(String),
            `grams` UInt64,
            `id` UInt64,
            `option1` String,
            `option2` Nullable(String),
            `option3` Nullable(String),
            `position` UInt64,
            `price` String,
            `product_id` UInt64,
            `requires_shipping` UInt8,
            `sku` Nullable(String),
            `taxable` UInt8,
            `title` String,
            `updated_at` DateTime64(3)
        ),
        `vendor` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();

SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS test (
    `products` Nested(
        `body_html` String,
        `created_at` String,
        `handle` String,
        `id` UInt64,
        `images` Nested(
            `created_at` String,
            `height` UInt64,
            `id` UInt64,
            `position` UInt64,
            `product_id` UInt64,
            `src` String,
            `updated_at` String,
            `variant_ids` Array(String),
            `width` UInt64
        ),
        `options` Nested(
            `name` String,
            `position` UInt64,
            `values` Array(String)
        ),
        `product_type` String,
        `published_at` String,
        `tags` Array(String),
        `title` String,
        `updated_at` String,
        `variants` Nested(
            `available` UInt8,
            `compare_at_price` Nullable(String),
            `created_at` String,
            `featured_image` Nullable(String),
            `grams` UInt64,
            `id` UInt64,
            `option1` String,
            `option2` Nullable(String),
            `option3` Nullable(String),
            `position` UInt64,
            `price` String,
            `product_id` UInt64,
            `requires_shipping` UInt8,
            `sku` Nullable(String),
            `taxable` UInt8,
            `title` String,
            `updated_at` String
        ),
        `vendor` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();

SELECT * FROM file('/var/lib/clickhouse/user_files/2025-03-01_7-ELEVEN-INC_index.json', 'JSONEachRow');

SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS eleven_inc_index (
    `reporting_entity_name` String,
    `reporting_entity_type` String,
    `reporting_structure` Nested(
        `allowed_amount_file` Nested(
            `description` String,
            `location` String
        ),
        `in_network_files` Array(String),
        `reporting_plans` Nested(
            `plan_id` UInt64,
            `plan_id_type` String,
            `plan_market_type` String,
            `plan_name` String
        )
    )
) ENGINE = MergeTree()
ORDER BY tuple();

SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS eleven_inc_index (
    reporting_entity_name String,
    reporting_entity_type String,
    reporting_structure Array(
        Tuple(
            allowed_amount_file Tuple(
                description Nullable(String),
                location Nullable(String)
            ),
            in_network_files Array(Tuple(
                description Nullable(String),
                location Nullable(String)
            )),
            reporting_plans Array(Tuple(
                plan_id String,
                plan_id_type Nullable(String),
                plan_market_type Nullable(String),
                plan_name Nullable(String)
            ))
        )
    )
) ENGINE = MergeTree()
ORDER BY tuple();


INSERT INTO eleven_inc_index
SELECT *
FROM file(
    '/var/lib/clickhouse/user_files/2025-03-01_7-ELEVEN-INC_index.json',
    'JSONEachRow'
);

SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS eleven_inc_index
(
    `reporting_entity_name` String,
    `reporting_entity_type` String,
    `reporting_structure` Nested(
        `reporting_plan_id` Nullable(String),
        `reporting_plan_id_type` Nullable(String),
        `ids` Nullable(Array(Int8)),
        `reporting_plans` Nested(
            `plan_name` String,
            `plan_id` String,
            `plan_id_type` String,
            `plan_market_type` String,
        ),
        `in_network_files` Nullable(Array(String)),
        `allowed_amount_file` Nullable(String)
    )
)
ENGINE = MergeTree()
ORDER BY tuple();


SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS eleven_inc_index
(
    `reporting_entity_name` String,
    `reporting_entity_type` String,
    `reporting_structure` Nested(
        `reporting_plan_id` Nullable(String),
        `reporting_plan_id_type` Nullable(String),
        `ids` Nullable(Array(Int8)),
        `reporting_plans` Nested(
            `plan_name` String,
            `plan_id` String,
            `plan_id_type` String,
            `plan_market_type` String
        ),
        `in_network_files` Nullable(Array(String)),
        `allowed_amount_file` Nullable(Nested(
            `description` String,
            `location` String
        ))
    )
)
ENGINE = MergeTree()
ORDER BY tuple();


SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS eleven_inc_index
(
    `reporting_entity_name` String,
    `reporting_entity_type` String,
    `reporting_structure` Nested(
        `reporting_plan_id` Nullable(String),
        `reporting_plan_id_type` Nullable(String),
        `ids` Nullable(Array(Int8)),
        `reporting_plans` Nested(
            `plan_name` String,
            `plan_id` String,
            `plan_id_type` String,
            `plan_market_type` String
        ),
        `in_network_files` Nullable(Array(String)),
        `allowed_amount_file` Nullable(Nested(
            `description` String,
            `location` String
        ))
    )
)
ENGINE = MergeTree()
ORDER BY tuple();


SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS eleven_inc_index
(
    `products` Nested(
        `id` Int32,
        `title` String,
        `handle` String,
        `body_html` String,
        `published_at` DateTime,
        `created_at` DateTime,
        `updated_at` DateTime,
        `vendor` String,
        `product_type` String,
        `tags` Array(String),
        `variants` Nested(
            `id` Int64,
            `title` String,
            `option1` String,
            `option2` Nullable(String),
            `option3` Nullable(String),
            `sku` Nullable(String),
            `requires_shipping` UInt8,
            `taxable` UInt8,
            `featured_image` Nullable(String),
            `available` UInt8,
            `price` String,
            `grams` Int16, ------
            `compare_at_price` Nullable(String),
            `position` Int8,
            `product_id` Int64,
            `created_at` DateTime,
            `updated_at` DateTime
        ),
        `images` Nested(
            `id` Int64,
            `created_at` DateTime,
            `position` Int8,
            `updated_at` DateTime,
            `product_id` Int64,
            `variant_ids` Nullable(Array(String)),
            `src` String,
            `width` Int16,
            `height` Int16
        ),
        `options` Nested(
            `name` String,
            `position` Int8,
            `values` Array(String)
        )
    )
)
ENGINE = MergeTree()
ORDER BY tuple();
