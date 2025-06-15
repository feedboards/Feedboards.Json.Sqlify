-- Top-level entity
CREATE TABLE reporting_entity (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    type VARCHAR(100) NOT NULL
);

-- Each reporting_structure entry
CREATE TABLE reporting_structure (
    id SERIAL PRIMARY KEY,
    reporting_entity_id INT REFERENCES reporting_entity(id),
    reporting_plan_id VARCHAR(50),
    reporting_plan_id_type VARCHAR(50)
);

-- Plans under each reporting structure
CREATE TABLE reporting_plan (
    id SERIAL PRIMARY KEY,
    reporting_structure_id INT REFERENCES reporting_structure(id),
    plan_name VARCHAR(255),
    plan_id VARCHAR(50),
    plan_id_type VARCHAR(50),
    plan_market_type VARCHAR(50)
);

-- In-network files for each structure
CREATE TABLE in_network_file (
    id SERIAL PRIMARY KEY,
    reporting_structure_id INT REFERENCES reporting_structure(id),
    description TEXT,
    location TEXT
);

-- Allowed amount file (only one per structure)
CREATE TABLE allowed_amount_file (
    id SERIAL PRIMARY KEY,
    reporting_structure_id INT REFERENCES reporting_structure(id),
    description TEXT,
    location TEXT
);