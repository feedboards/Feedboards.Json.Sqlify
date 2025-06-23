CREATE TABLE root_reporting_structure_reporting_plans (
    id SERIAL PRIMARY KEY,
    plan_name VARCHAR,
    plan_id VARCHAR,
    plan_id_type VARCHAR,
    plan_market_type VARCHAR,
    root_reporting_structure_reporting_plans_parent_id IN
);

CREATE TABLE root_reporting_structure_allowed_amount_file (
    id SERIAL PRIMARY KEY,
    description VARCHAR,
    location VARCHAR,
    root_reporting_structure_allowed_amount_file_parent_id IN
);

CREATE TABLE root_reporting_structure (
    id SERIAL PRIMARY KEY,
    reporting_plan_id VARCHAR,
    reporting_plan_id_type VARCHAR,
    allowed_amount_file_id INT,
    root_reporting_structure_parent_id IN
);

CREATE TABLE root_reporting_structure_in_network_files (
    id SERIAL PRIMARY KEY,
    description VARCHAR,
    location VARCHAR,
    root_reporting_structure_in_network_files_parent_id IN
);

CREATE TABLE root (
    id SERIAL PRIMARY KEY,
    reporting_entity_name VARCHAR,
    reporting_entity_type VARCHA
);
