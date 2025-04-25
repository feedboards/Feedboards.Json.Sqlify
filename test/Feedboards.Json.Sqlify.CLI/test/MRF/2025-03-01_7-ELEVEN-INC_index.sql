SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS mrf_index
(
    `reporting_entity_name` String,
    `reporting_entity_type` String,
    `reporting_structure` Nested(
        `reporting_plan_id` Nullable(String),
        `reporting_plan_id_type` Nullable(String),
        `ids` Array(Int8),
        `reporting_plans` Nested(
            `plan_name` String,
            `plan_id` String,
            `plan_id_type` String,
            `plan_market_type` String
        ),
        `in_network_files` Nested(
            `description` String,
            `location` String
        ),
        `allowed_amount_file` Nullable(Nested(
            `description` String,
            `location` String
        ))
    )
)
ENGINE = MergeTree()
ORDER BY tuple();
