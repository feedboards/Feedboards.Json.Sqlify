SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS products (
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
