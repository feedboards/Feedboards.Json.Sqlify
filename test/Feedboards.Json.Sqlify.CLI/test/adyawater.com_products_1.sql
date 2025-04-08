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
            `grams` Int16,
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
