CREATE TABLE IF NOT EXISTS simple (
    `test` Nested(
        `name` String,
        `title` String
    )
) ENGINE = MergeTree()
ORDER BY tuple();