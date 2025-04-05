CREATE TABLE IF NOT EXISTS eleven_inc_index
(
    `test` Nested(
   `name` String,
   `title` String,
)
)
ENGINE = MergeTree()
ORDER BY tuple();
