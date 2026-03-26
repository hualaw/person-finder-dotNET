DROP DATABASE IF EXISTS personfinder;
CREATE DATABASE personfinder;

\c personfinder

CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE persons (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    job_title VARCHAR(255),
    hobbies TEXT[],
    bio TEXT
);

CREATE TABLE locations (
    id BIGSERIAL PRIMARY KEY,
    person_id BIGINT NOT NULL UNIQUE,
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    geom GEOGRAPHY(POINT, 4326),
    CONSTRAINT fk_person
        FOREIGN KEY(person_id)
        REFERENCES persons(id)
        ON DELETE CASCADE
);

CREATE INDEX idx_locations_geom ON locations USING GIST (geom);

INSERT INTO persons (id, name, job_title, hobbies, bio)
VALUES
    (1, 'Alice Chen', 'Software Engineer', ARRAY['hiking', 'photography', 'board games'], 'Backend engineer who enjoys building reliable systems and exploring regional hiking trails.'),
    (2, 'Ben Carter', 'Product Designer', ARRAY['illustration', 'coffee', 'cycling'], 'Product designer focused on simple user flows, visual polish, and strong collaboration with engineers.'),
    (3, 'Chloe Singh', 'Data Analyst', ARRAY['running', 'puzzles', 'travel'], 'Data analyst who turns messy operational data into useful product and growth insights.'),
    (4, 'Daniel Kim', 'DevOps Engineer', ARRAY['gaming', 'automation', 'weightlifting'], 'DevOps engineer interested in platform tooling, deployment pipelines, and resilient infrastructure.'),
    (5, 'Eva Martinez', 'Marketing Manager', ARRAY['cooking', 'yoga', 'podcasts'], 'Marketing manager with a strong interest in customer storytelling and brand strategy.'),
    (6, 'Felix Brown', 'Mobile Developer', ARRAY['climbing', 'music', 'reading'], 'Mobile developer who enjoys performance tuning and creating polished cross-platform experiences.'),
    (7, 'Grace Liu', 'HR Specialist', ARRAY['volunteering', 'painting', 'gardening'], 'People-focused HR specialist who cares about team health, hiring quality, and internal culture.'),
    (8, 'Henry Wilson', 'Sales Lead', ARRAY['golf', 'networking', 'cooking'], 'Sales lead with a practical approach to relationship building and pipeline discipline.'),
    (9, 'Isla Thompson', 'QA Engineer', ARRAY['knitting', 'escape rooms', 'baking'], 'QA engineer who likes finding edge cases early and improving product release confidence.'),
    (10, 'Jack Robinson', 'Operations Manager', ARRAY['fishing', 'woodworking', 'rugby'], 'Operations manager focused on process clarity, delivery coordination, and steady execution.');

INSERT INTO locations (id, person_id, latitude, longitude, geom)
VALUES
    (1, 1, -36.848500, 174.763300, ST_SetSRID(ST_MakePoint(174.763300, -36.848500), 4326)::geography),
    (2, 2, -36.850100, 174.765000, ST_SetSRID(ST_MakePoint(174.765000, -36.850100), 4326)::geography),
    (3, 3, -36.847200, 174.761400, ST_SetSRID(ST_MakePoint(174.761400, -36.847200), 4326)::geography),
    (4, 4, -36.852300, 174.770200, ST_SetSRID(ST_MakePoint(174.770200, -36.852300), 4326)::geography),
    (5, 5, -36.845900, 174.758700, ST_SetSRID(ST_MakePoint(174.758700, -36.845900), 4326)::geography),
    (6, 6, -36.853800, 174.767600, ST_SetSRID(ST_MakePoint(174.767600, -36.853800), 4326)::geography),
    (7, 7, -36.849400, 174.756900, ST_SetSRID(ST_MakePoint(174.756900, -36.849400), 4326)::geography),
    (8, 8, -36.844600, 174.764800, ST_SetSRID(ST_MakePoint(174.764800, -36.844600), 4326)::geography),
    (9, 9, -36.851700, 174.759500, ST_SetSRID(ST_MakePoint(174.759500, -36.851700), 4326)::geography),
    (10, 10, -36.846800, 174.769100, ST_SetSRID(ST_MakePoint(174.769100, -36.846800), 4326)::geography);

SELECT setval('persons_id_seq', (SELECT MAX(id) FROM persons));
SELECT setval('locations_id_seq', (SELECT MAX(id) FROM locations));