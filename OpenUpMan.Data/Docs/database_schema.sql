-- Usuarios
CREATE TABLE users (
                       id INTEGER PRIMARY KEY AUTOINCREMENT,
                       username TEXT NOT NULL UNIQUE,
                       password_hash TEXT NOT NULL,
                       created_at TEXT DEFAULT (datetime('now'))
);

-- Permisos
CREATE TABLE permissions (
                           id INTEGER PRIMARY KEY AUTOINCREMENT,
                           name TEXT NOT NULL UNIQUE,
                           description TEXT
);

-- Relación muchos a muchos entre roles y permisos
CREATE TABLE role_permissions (
                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                               role_id INTEGER NOT NULL,
                               permission_id INTEGER NOT NULL,
                               FOREIGN KEY(role_id) REFERENCES roles(id),
                               FOREIGN KEY(permission_id) REFERENCES permissions(id)
);

-- Roles globales (PO, SM, developer, tester, admin, creador...)
CREATE TABLE roles (
                       id INTEGER PRIMARY KEY AUTOINCREMENT,
                       name TEXT NOT NULL UNIQUE,
                       description TEXT
);


-- Usuarios por proyecto con rol
CREATE TABLE project_users (
                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                               project_id INTEGER NOT NULL,
                               user_id INTEGER NOT NULL,
                               role_id INTEGER NOT NULL,
                               added_at TEXT DEFAULT (datetime('now')),
                               FOREIGN KEY(project_id) REFERENCES projects(id),
                               FOREIGN KEY(user_id) REFERENCES users(id),
                               FOREIGN KEY(role_id) REFERENCES roles(id)
);


-- Proyectos
CREATE TABLE projects (
                          id INTEGER PRIMARY KEY AUTOINCREMENT,
                          name TEXT NOT NULL,
                          code TEXT UNIQUE,
                          description TEXT,
                          start_date TEXT,
                          status TEXT DEFAULT 'CREATED', -- CREATED, ACTIVE, CLOSED
                          created_by INTEGER,
                          created_at TEXT DEFAULT (datetime('now')),
                          updated_at TEXT,
                          deleted_at TEXT,
                          FOREIGN KEY(created_by) REFERENCES users(id)
);


-- Fases: una fila por fase por proyecto (se generan 4 por defecto)
CREATE TABLE phases (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        project_id INTEGER NOT NULL,
                        name TEXT NOT NULL, -- Inception, Elaboration, Construction, Transition
                        start_date TEXT,
                        end_date TEXT,
                        status TEXT DEFAULT 'PENDING', -- PENDING, ACTIVE, COMPLETED
                        order_index INTEGER,
                        FOREIGN KEY(project_id) REFERENCES projects(id)
);


-- Iteraciones dentro de una fase
CREATE TABLE iterations (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            phase_id INTEGER NOT NULL,
                            name TEXT,
                            goal TEXT,
                            start_date TEXT,
                            end_date TEXT,
                            completion_percentage INTEGER DEFAULT 0,
                            FOREIGN KEY(phase_id) REFERENCES phases(id)
);


-- Microincrementos vinculados a una iteración
CREATE TABLE microincrements (
                                 id INTEGER PRIMARY KEY AUTOINCREMENT,
                                 iteration_id INTEGER NOT NULL,
                                 title TEXT NOT NULL,
                                 description TEXT,
                                 date TEXT DEFAULT (datetime('now')),
                                 author_id INTEGER,
                                 type TEXT DEFAULT 'functional', -- functional | technical
                                 artifact_id INTEGER, -- opcional
                                 evidence_url TEXT,
                                 FOREIGN KEY(iteration_id) REFERENCES iterations(id),
                                 FOREIGN KEY(author_id) REFERENCES users(id),
                                 FOREIGN KEY(artifact_id) REFERENCES artifacts(id)
);


-- Artefactos asociados a proyecto y fase
CREATE TABLE artifacts (
                           id INTEGER PRIMARY KEY AUTOINCREMENT,
                           project_id INTEGER NOT NULL,
                           phase_id INTEGER NOT NULL,
                           name TEXT NOT NULL,
                           artifact_type TEXT, -- libre: Vision, UseCaseModel, ArchitectureDoc, SourceCode...
                           mandatory INTEGER DEFAULT 0, -- 0 = opcional, 1 = obligatorio
                           description TEXT,
                           current_state TEXT DEFAULT 'PENDING' -- PENDING, DELIVERED
);

-- Versiones de artefacto
CREATE TABLE artifact_versions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    artifact_id INTEGER NOT NULL,
    version_number INTEGER NOT NULL DEFAULT 0, -- will be set by trigger when 0
    created_by INTEGER,
    created_at TEXT DEFAULT (datetime('now')),
    notes TEXT,
    file_blob BLOB,
    file_mime TEXT,
    build_info TEXT,
    FOREIGN KEY(artifact_id) REFERENCES artifacts(id),
    FOREIGN KEY(created_by) REFERENCES users(id),
    UNIQUE(artifact_id, version_number)
);

CREATE TRIGGER artifact_versions_set_version_number
AFTER INSERT ON artifact_versions
BEGIN
    UPDATE artifact_versions
    SET version_number =
        (SELECT COALESCE(MAX(version_number), 0) + 1
         FROM artifact_versions
         WHERE artifact_id = NEW.artifact_id
           AND id <> NEW.id)
    WHERE id = NEW.id
      AND (NEW.version_number IS NULL OR NEW.version_number = 0);
END;