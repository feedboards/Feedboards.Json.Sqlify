SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS eleven_inc_index
(
    `products` Nested(
        `id` Int32,
        `title` String,
        `handle` String,
        `body_html` String,
        `published_at` String,
        `created_at` String,
        `updated_at` String,
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
            `grams` Int16,
            `compare_at_price` Nullable(String),
            `position` Int8,
            `product_id` Int64,
            `created_at` String,
            `updated_at` String
        ),
        `images` Nested(
            `id` Int64,
            `created_at` String,
            `position` Int8,
            `updated_at` String,
            `product_id` Int64,
            `variant_ids` Array(String),
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
