SET flatten_nested=0;

CREATE TABLE IF NOT EXISTS eleven_inc_index
(
    `reporting_entity_name` String,
    `reporting_entity_type` String,
    `reporting_structure` Array((Tuple(`reporting_plans` Array(Tuple(`plan_name` String, `plan_id` String, `plan_id_type` String, `plan_market_type` String)), `in_network_files` Array(String), `allowed_amount_file` Nullable(Tuple(`description` String, `location` String))))
)
ENGINE = MergeTree()
ORDER BY tuple();
