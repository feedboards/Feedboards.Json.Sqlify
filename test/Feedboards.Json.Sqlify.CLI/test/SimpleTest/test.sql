CREATE TABLE IF NOT EXISTS eleven_inc_index
(
    `message` String,
    `metric` Float32,
    `timestamp` DateTime,
    `user_id` Int8
)
ENGINE = MergeTree()
ORDER BY tuple();
