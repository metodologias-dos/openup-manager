-- Estructura de Base de Datos para OpenUpMan
-- SQLite Database Schema

-- ====================================
-- TABLA: users
-- ====================================
CREATE TABLE users (
    Id                  TEXT PRIMARY KEY,         -- Guid
    Username            TEXT NOT NULL UNIQUE,
    PasswordHash        TEXT NOT NULL,
    CreatedAt           TEXT NOT NULL,            -- ISO 8601
    PasswordChangedAt   TEXT                      -- nullable
);

-- ====================================
-- TABLA: rol
-- ====================================
CREATE TABLE rol (
    Id                  TEXT PRIMARY KEY,         -- Guid
    Name                TEXT NOT NULL,            -- AUTOR, REVISOR, PO, SM, DESARROLLADOR, TESTER, ADMIN
    Description         TEXT
);

-- ====================================
-- TABLA: permission
-- ====================================
CREATE TABLE permission (
    Id                  TEXT PRIMARY KEY,         -- Guid
    Name                TEXT NOT NULL,            -- BorrarProyecto, RenombrarProyecto, etc.
    Description         TEXT
);

-- ====================================
-- TABLA: rol_permission
-- ====================================
CREATE TABLE rol_permission (
    RoleId              TEXT NOT NULL,            -- FK -> rol.Id
    PermissionId        TEXT NOT NULL,            -- FK -> permission.Id
    PRIMARY KEY (RoleId, PermissionId),
    CONSTRAINT fk_rp_role FOREIGN KEY (RoleId) REFERENCES rol(Id),
    CONSTRAINT fk_rp_permission FOREIGN KEY (PermissionId) REFERENCES permission(Id)
);

-- ====================================
-- TABLA: projects
-- ====================================
CREATE TABLE projects (
    Id              TEXT PRIMARY KEY,                -- Guid
    Identifier      TEXT NOT NULL UNIQUE,            -- ej. "PROY-001"
    Name            TEXT NOT NULL,
    Description     TEXT,
    StartDate       TEXT NOT NULL,                   -- ISO datetime
    OwnerId         TEXT NOT NULL,                   -- FK -> users.Id
    State           TEXT NOT NULL DEFAULT 'CREATED', -- CREATED, ACTIVE, ARCHIVED, CLOSED
    CreatedAt       TEXT NOT NULL,
    UpdatedAt       TEXT,
    CONSTRAINT fk_projects_owner FOREIGN KEY (OwnerId) REFERENCES users(Id),
    CONSTRAINT chk_projects_state CHECK (
        State IN ('CREATED','ACTIVE','ARCHIVED','CLOSED')
    )
);

-- ====================================
-- TABLA: project_users
-- ====================================
CREATE TABLE project_users (
    ProjectId   TEXT NOT NULL,     -- FK -> projects.Id
    UserId      TEXT NOT NULL,     -- FK -> users.Id
    RoleId      TEXT NOT NULL,     -- FK -> rol.Id
    PRIMARY KEY (ProjectId, UserId),
    CONSTRAINT fk_pu_project FOREIGN KEY (ProjectId) REFERENCES projects(Id),
    CONSTRAINT fk_pu_user FOREIGN KEY (UserId) REFERENCES users(Id),
    CONSTRAINT fk_pu_role FOREIGN KEY (RoleId) REFERENCES rol(Id)
);

-- ====================================
-- TABLA: project_phases
-- ====================================
CREATE TABLE project_phases (
    Id          TEXT PRIMARY KEY,      -- Guid
    ProjectId   TEXT NOT NULL,         -- FK -> projects.Id
    Code        TEXT NOT NULL,         -- INCEPTION, ELABORATION, CONSTRUCTION, TRANSITION
    Name        TEXT NOT NULL,
    Order       INTEGER NOT NULL,
    State       TEXT NOT NULL DEFAULT 'NOT_STARTED', -- NOT_STARTED, IN_PROGRESS, COMPLETED
    CONSTRAINT fk_pp_project FOREIGN KEY (ProjectId) REFERENCES projects(Id),
    CONSTRAINT chk_pp_code CHECK (Code IN ('INCEPTION','ELABORATION','CONSTRUCTION','TRANSITION')),
    CONSTRAINT chk_pp_state CHECK (State IN ('NOT_STARTED','IN_PROGRESS','COMPLETED'))
);

-- ====================================
-- TABLA: phase_items
-- ====================================
CREATE TABLE phase_items (
    Id                  TEXT PRIMARY KEY,      -- Guid
    ProjectPhaseId      TEXT NOT NULL,         -- FK -> project_phases.Id
    Type                TEXT NOT NULL,         -- 'ITERATION' o 'MICROINCREMENT'
    Number              INTEGER NOT NULL,
    ParentIterationId   TEXT,                  -- FK -> phase_items.Id (iteración padre)
    Name                TEXT NOT NULL,         -- nombre de la iteración o microincremento
    Description         TEXT,                  -- descripción detallada
    StartDate           TEXT,                  -- para iteración; para microincremento puede ser solo fecha evento
    EndDate             TEXT,
    State               TEXT NOT NULL DEFAULT 'PLANNED', -- PLANNED, ACTIVE, DONE, CANCELLED...
    CreatedBy           TEXT NOT NULL,         -- FK -> users.Id
    CreatedAt           TEXT NOT NULL,
    CONSTRAINT fk_pi_phase FOREIGN KEY (ProjectPhaseId) REFERENCES project_phases(Id),
    CONSTRAINT fk_pi_parent FOREIGN KEY (ParentIterationId) REFERENCES phase_items(Id),
    CONSTRAINT fk_pi_creator FOREIGN KEY (CreatedBy) REFERENCES users(Id),
    CONSTRAINT chk_pi_type CHECK (Type IN ('ITERATION','MICROINCREMENT')),
    CONSTRAINT chk_pi_state CHECK (State IN ('PLANNED','ACTIVE','DONE','CANCELLED'))
);

-- ====================================
-- TABLA: phase_item_users
-- ====================================
CREATE TABLE phase_item_users (
    PhaseItemId TEXT NOT NULL,         -- FK -> phase_items.Id
    UserId      TEXT NOT NULL,         -- FK -> users.Id
    Role        TEXT NOT NULL DEFAULT 'PARTICIPANT', -- PARTICIPANT, RESPONSIBLE, etc.
    PRIMARY KEY (PhaseItemId, UserId),
    CONSTRAINT fk_piu_item FOREIGN KEY (PhaseItemId) REFERENCES phase_items(Id),
    CONSTRAINT fk_piu_user FOREIGN KEY (UserId) REFERENCES users(Id)
);

-- ====================================
-- TABLA: documents
-- ====================================
CREATE TABLE documents (
    Id                  TEXT PRIMARY KEY,      -- Guid
    PhaseItemId         TEXT NOT NULL,         -- FK -> phase_items.Id
    Title               TEXT NOT NULL,
    Description         TEXT,
    CreatedBy           TEXT NOT NULL,         -- FK -> users.Id
    CreatedAt           TEXT NOT NULL,
    LastVersionNumber   INTEGER NOT NULL DEFAULT 0, -- para saber en qué versión va (1,2,3...)
    CONSTRAINT fk_docs_phase FOREIGN KEY (PhaseItemId) REFERENCES phase_items(Id),
    CONSTRAINT fk_docs_creator FOREIGN KEY (CreatedBy) REFERENCES users(Id)
);

-- ====================================
-- TABLA: document_versions
-- ====================================
CREATE TABLE document_versions (
    Id                      TEXT PRIMARY KEY,      -- Guid
    DocumentId              TEXT NOT NULL,         -- FK -> documents.Id
    VersionNumber           INTEGER NOT NULL,      -- 1,2,3...
    CreatedAt               TEXT NOT NULL,
    CreatedBy               TEXT NOT NULL,         -- FK -> users.Id
    FilePath                TEXT NOT NULL,         -- ruta al archivo en el sistema
    Observations            TEXT,                  -- descripción de cambios
    CONSTRAINT fk_dv_doc FOREIGN KEY (DocumentId) REFERENCES documents(Id),
    CONSTRAINT fk_dv_creator FOREIGN KEY (CreatedBy) REFERENCES users(Id),
    CONSTRAINT uq_dv_version UNIQUE (DocumentId, VersionNumber)
);

-- ====================================
-- TABLA: artefacts
-- ====================================
CREATE TABLE artefacts (
    Id                  TEXT PRIMARY KEY,      -- Guid
    Name                TEXT NOT NULL,
    Description         TEXT
);

-- ====================================
-- TABLA: phase_artefacts
-- ====================================
CREATE TABLE phase_artefacts (
    PhaseId             TEXT NOT NULL,         -- FK -> project_phases.Id
    ArtefactId          TEXT NOT NULL,         -- FK -> artefacts.Id
    DocumentId          TEXT,                  -- FK -> documents.Id (nullable)
    Registrado          INTEGER NOT NULL DEFAULT 0, -- 0 = no, 1 = sí
    PRIMARY KEY (PhaseId, ArtefactId),
    CONSTRAINT fk_pa_phase FOREIGN KEY (PhaseId) REFERENCES project_phases(Id),
    CONSTRAINT fk_pa_artefact FOREIGN KEY (ArtefactId) REFERENCES artefacts(Id),
    CONSTRAINT fk_pa_document FOREIGN KEY (DocumentId) REFERENCES documents(Id)
);


