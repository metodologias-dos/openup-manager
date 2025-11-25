-- Estructura de Base de Datos para OpenUpMan
-- SQLite Database Schema

-- ====================================
-- TABLA: users
-- ====================================
CREATE TABLE users (
    id                  TEXT PRIMARY KEY,         -- Guid
    username            TEXT NOT NULL UNIQUE,
    password_hash       TEXT NOT NULL,
    created_at          TEXT NOT NULL,            -- ISO 8601
    password_changed_at TEXT                      -- nullable
);

-- ====================================
-- TABLA: projects
-- ====================================
CREATE TABLE projects (
    id              TEXT PRIMARY KEY,                -- Guid
    identifier      TEXT NOT NULL UNIQUE,            -- ej. "PROY-001"
    name            TEXT NOT NULL,
    description     TEXT,
    start_date      TEXT NOT NULL,                   -- ISO datetime
    owner_id        TEXT NOT NULL,                   -- FK -> users.id
    state           TEXT NOT NULL DEFAULT 'CREATED', -- CREATED, ACTIVE, ARCHIVED, CLOSED
    created_at      TEXT NOT NULL,
    updated_at      TEXT,
    CONSTRAINT fk_projects_owner FOREIGN KEY (owner_id) REFERENCES users(id),
    CONSTRAINT chk_projects_state CHECK (
        state IN ('CREATED','ACTIVE','ARCHIVED','CLOSED')
    )
);

-- ====================================
-- TABLA: project_users
-- ====================================
CREATE TABLE project_users (
    project_id  TEXT NOT NULL,     -- FK -> projects.id
    user_id     TEXT NOT NULL,     -- FK -> users.id
    role        TEXT NOT NULL DEFAULT 'VIEWER', -- VIEWER, EDITOR, OWNER
    PRIMARY KEY (project_id, user_id),
    CONSTRAINT fk_pu_project FOREIGN KEY (project_id) REFERENCES projects(id),
    CONSTRAINT fk_pu_user FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT chk_pu_role CHECK (role IN ('VIEWER','EDITOR','OWNER'))
);

-- ====================================
-- TABLA: project_phases
-- ====================================
CREATE TABLE project_phases (
    id          TEXT PRIMARY KEY,      -- Guid
    project_id  TEXT NOT NULL,         -- FK -> projects.id
    code        TEXT NOT NULL,         -- INCEPTION, ELABORATION, CONSTRUCTION, TRANSITION
    name        TEXT NOT NULL,
    order       INTEGER NOT NULL,
    state       TEXT NOT NULL DEFAULT 'NOT_STARTED', -- NOT_STARTED, IN_PROGRESS, COMPLETED
    CONSTRAINT fk_pp_project FOREIGN KEY (project_id) REFERENCES projects(id),
    CONSTRAINT chk_pp_code CHECK (code IN ('INCEPTION','ELABORATION','CONSTRUCTION','TRANSITION')),
    CONSTRAINT chk_pp_state CHECK (state IN ('NOT_STARTED','IN_PROGRESS','COMPLETED'))
);

-- ====================================
-- TABLA: phase_items
-- ====================================
CREATE TABLE phase_items (
    id                  TEXT PRIMARY KEY,      -- Guid
    project_phase_id    TEXT NOT NULL,         -- FK -> project_phases.id
    type                TEXT NOT NULL,         -- 'ITERATION' o 'MICROINCREMENT'
    number              INTEGER NOT NULL,
    parent_iteration_id TEXT,                  -- FK -> phase_items.id (iteración padre)
    name                TEXT NOT NULL,         -- nombre de la iteración o microincremento
    description         TEXT,                  -- descripción detallada
    start_date          TEXT,                  -- para iteración; para microincremento puede ser solo fecha evento
    end_date            TEXT,
    state               TEXT NOT NULL DEFAULT 'PLANNED', -- PLANNED, ACTIVE, DONE, CANCELLED...
    created_by          TEXT NOT NULL,         -- FK -> users.id
    created_at          TEXT NOT NULL,
    CONSTRAINT fk_pi_phase FOREIGN KEY (project_phase_id) REFERENCES project_phases(id),
    CONSTRAINT fk_pi_parent FOREIGN KEY (parent_iteration_id) REFERENCES phase_items(id),
    CONSTRAINT fk_pi_creator FOREIGN KEY (created_by) REFERENCES users(id),
    CONSTRAINT chk_pi_type CHECK (type IN ('ITERATION','MICROINCREMENT')),
    CONSTRAINT chk_pi_state CHECK (state IN ('PLANNED','ACTIVE','DONE','CANCELLED'))
);

-- ====================================
-- TABLA: phase_item_users
-- ====================================
CREATE TABLE phase_item_users (
    phase_item_id   TEXT NOT NULL,         -- FK -> phase_items.id
    user_id         TEXT NOT NULL,         -- FK -> users.id
    role            TEXT NOT NULL DEFAULT 'PARTICIPANT', -- PARTICIPANT, RESPONSIBLE, etc.
    PRIMARY KEY (phase_item_id, user_id),
    CONSTRAINT fk_piu_item FOREIGN KEY (phase_item_id) REFERENCES phase_items(id),
    CONSTRAINT fk_piu_user FOREIGN KEY (user_id) REFERENCES users(id)
);

-- ====================================
-- TABLA: documents
-- ====================================
CREATE TABLE documents (
    id                  TEXT PRIMARY KEY,      -- Guid
    phase_items_id      TEXT NOT NULL,         -- FK -> phase_items.id
    title               TEXT NOT NULL,
    description         TEXT,
    created_by          TEXT NOT NULL,         -- FK -> users.id
    created_at          TEXT NOT NULL,
    last_version_number INTEGER NOT NULL DEFAULT 0, -- para saber en qué versión va (1,2,3...)
    CONSTRAINT fk_docs_phase FOREIGN KEY (phase_items_id) REFERENCES phase_items(id),
    CONSTRAINT fk_docs_creator FOREIGN KEY (created_by) REFERENCES users(id)
);

-- ====================================
-- TABLA: document_versions
-- ====================================
CREATE TABLE document_versions (
    id                      TEXT PRIMARY KEY,      -- Guid
    document_id             TEXT NOT NULL,         -- FK -> documents.id
    version_number          INTEGER NOT NULL,      -- 1,2,3...
    created_at              TEXT NOT NULL,
    created_by              TEXT NOT NULL,         -- FK -> users.id
    file_path               TEXT NOT NULL,         -- ruta al archivo en el sistema
    observations            TEXT,                  -- descripción de cambios
    CONSTRAINT fk_dv_doc FOREIGN KEY (document_id) REFERENCES documents(id),
    CONSTRAINT fk_dv_creator FOREIGN KEY (created_by) REFERENCES users(id),
    CONSTRAINT uq_dv_version UNIQUE (document_id, version_number)
);

