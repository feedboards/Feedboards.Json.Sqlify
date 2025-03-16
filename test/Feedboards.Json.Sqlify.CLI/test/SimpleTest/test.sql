CREATE TABLE IF NOT EXISTS test (
    `message` String,
    `metric` Float64,
    `timestamp` DateTime64(3),
    `user_id` UInt64
) ENGINE = MergeTree()
ORDER BY tuple();