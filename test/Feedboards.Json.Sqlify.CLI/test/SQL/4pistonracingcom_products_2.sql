CREATE TABLE IF NOT EXISTS 4pistonracingcom_products_2 (
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
        `tags` Array(String),
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
            `sku` String,
            `taxable` UInt8,
            `title` String,
            `updated_at` DateTime64(3)
        ),
        `vendor` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();