using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpenUpMan.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithRolesAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    username = table.Column<string>(type: "TEXT", nullable: false),
                    password_hash = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    role_id = table.Column<int>(type: "INTEGER", nullable: false),
                    permission_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    code = table.Column<string>(type: "TEXT", nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    start_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.id);
                    table.ForeignKey(
                        name: "FK_projects_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "phases",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    project_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    start_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    end_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: false),
                    order_index = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phases", x => x.id);
                    table.ForeignKey(
                        name: "FK_phases_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    project_id = table.Column<int>(type: "INTEGER", nullable: false),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    role_id = table.Column<int>(type: "INTEGER", nullable: false),
                    added_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_project_users_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_project_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artifacts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    project_id = table.Column<int>(type: "INTEGER", nullable: false),
                    phase_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    artifact_type = table.Column<string>(type: "TEXT", nullable: true),
                    mandatory = table.Column<int>(type: "INTEGER", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    current_state = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artifacts", x => x.id);
                    table.ForeignKey(
                        name: "FK_artifacts_phases_phase_id",
                        column: x => x.phase_id,
                        principalTable: "phases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_artifacts_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "iterations",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    phase_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    goal = table.Column<string>(type: "TEXT", nullable: true),
                    start_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    end_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    completion_percentage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_iterations", x => x.id);
                    table.ForeignKey(
                        name: "FK_iterations_phases_phase_id",
                        column: x => x.phase_id,
                        principalTable: "phases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artifact_versions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    artifact_id = table.Column<int>(type: "INTEGER", nullable: false),
                    version_number = table.Column<int>(type: "INTEGER", nullable: false),
                    created_by = table.Column<int>(type: "INTEGER", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    notes = table.Column<string>(type: "TEXT", nullable: true),
                    file_blob = table.Column<byte[]>(type: "BLOB", nullable: true),
                    file_mime = table.Column<string>(type: "TEXT", nullable: true),
                    build_info = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artifact_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_artifact_versions_artifacts_artifact_id",
                        column: x => x.artifact_id,
                        principalTable: "artifacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_artifact_versions_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "microincrements",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    iteration_id = table.Column<int>(type: "INTEGER", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    author_id = table.Column<int>(type: "INTEGER", nullable: true),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    artifact_id = table.Column<int>(type: "INTEGER", nullable: true),
                    evidence_url = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_microincrements", x => x.id);
                    table.ForeignKey(
                        name: "FK_microincrements_artifacts_artifact_id",
                        column: x => x.artifact_id,
                        principalTable: "artifacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_microincrements_iterations_iteration_id",
                        column: x => x.iteration_id,
                        principalTable: "iterations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_microincrements_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Ver detalles del proyecto", "proyecto:ver" },
                    { 2, "Actualizar información del proyecto", "proyecto:actualizar" },
                    { 3, "Eliminar el proyecto", "proyecto:borrar" },
                    { 4, "Renombrar el proyecto", "proyecto:renombrar" },
                    { 5, "Cambiar estado del proyecto", "proyecto:cambiar-estado" },
                    { 6, "Agregar usuarios al proyecto", "usuarios:agregar" },
                    { 7, "Eliminar usuarios del proyecto", "usuarios:eliminar" },
                    { 8, "Ver usuarios del proyecto", "usuarios:ver" },
                    { 9, "Modificar roles de usuarios", "usuarios:modificar-roles" },
                    { 10, "Ver fases del proyecto", "fases:ver" },
                    { 11, "Crear nuevas fases", "fases:crear" },
                    { 12, "Actualizar información de fases", "fases:actualizar" },
                    { 13, "Avanzar a la siguiente fase", "fases:avanzar" },
                    { 14, "Cambiar estado de fase", "fases:cambiar-estado" },
                    { 15, "Ver iteraciones", "iteraciones:ver" },
                    { 16, "Crear nuevas iteraciones", "iteraciones:crear" },
                    { 17, "Actualizar iteraciones", "iteraciones:actualizar" },
                    { 18, "Avanzar/completar iteraciones", "iteraciones:avanzar" },
                    { 19, "Eliminar iteraciones", "iteraciones:eliminar" },
                    { 20, "Ver microincrementos", "microincrementos:ver" },
                    { 21, "Crear microincrementos", "microincrementos:crear" },
                    { 22, "Actualizar microincrementos", "microincrementos:actualizar" },
                    { 23, "Eliminar microincrementos", "microincrementos:eliminar" },
                    { 24, "Agregar documentos a microincrementos", "microincrementos:agregar-documentos" },
                    { 25, "Ver artefactos", "artefactos:ver" },
                    { 26, "Crear artefactos", "artefactos:crear" },
                    { 27, "Actualizar artefactos", "artefactos:actualizar" },
                    { 28, "Eliminar artefactos", "artefactos:eliminar" },
                    { 29, "Subir versiones de artefactos", "artefactos:subir-version" },
                    { 30, "Descargar artefactos", "artefactos:descargar" },
                    { 31, "Marcar artefactos como obligatorios", "artefactos:marcar-obligatorios" },
                    { 32, "Cambiar estado de artefactos", "artefactos:cambiar-estado" },
                    { 33, "Acceso de solo lectura a todo", "solo-lectura" },
                    { 34, "Gestionar artefactos mínimos", "artefactos:minimos" },
                    { 35, "Ver reportes y métricas", "reportes:ver" },
                    { 36, "Generar reportes personalizados", "reportes:generar" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Acceso completo al sistema, equivalente al Autor", "Administrador" },
                    { 2, "Gestión del producto y artefactos", "Product Owner" },
                    { 3, "Facilitador del equipo y gestión de microiteraciones", "Scrum Master" },
                    { 4, "Desarrollo y entrega de artefactos", "Desarrollador" },
                    { 5, "Pruebas y entrega de artefactos de testing", "Tester" },
                    { 6, "Solo lectura de todo el proyecto", "Revisor" },
                    { 7, "Creador del proyecto con todos los permisos", "Autor" },
                    { 8, "Visualización básica del proyecto", "Viewer" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "id", "permission_id", "role_id" },
                values: new object[,]
                {
                    { 1, 1, 7 },
                    { 2, 2, 7 },
                    { 3, 3, 7 },
                    { 4, 4, 7 },
                    { 5, 5, 7 },
                    { 6, 6, 7 },
                    { 7, 7, 7 },
                    { 8, 8, 7 },
                    { 9, 9, 7 },
                    { 10, 10, 7 },
                    { 11, 11, 7 },
                    { 12, 12, 7 },
                    { 13, 13, 7 },
                    { 14, 14, 7 },
                    { 15, 15, 7 },
                    { 16, 16, 7 },
                    { 17, 17, 7 },
                    { 18, 18, 7 },
                    { 19, 19, 7 },
                    { 20, 20, 7 },
                    { 21, 21, 7 },
                    { 22, 22, 7 },
                    { 23, 23, 7 },
                    { 24, 24, 7 },
                    { 25, 25, 7 },
                    { 26, 26, 7 },
                    { 27, 27, 7 },
                    { 28, 28, 7 },
                    { 29, 29, 7 },
                    { 30, 30, 7 },
                    { 31, 31, 7 },
                    { 32, 32, 7 },
                    { 33, 34, 7 },
                    { 34, 35, 7 },
                    { 35, 36, 7 },
                    { 36, 1, 1 },
                    { 37, 2, 1 },
                    { 38, 3, 1 },
                    { 39, 4, 1 },
                    { 40, 5, 1 },
                    { 41, 6, 1 },
                    { 42, 7, 1 },
                    { 43, 8, 1 },
                    { 44, 9, 1 },
                    { 45, 10, 1 },
                    { 46, 11, 1 },
                    { 47, 12, 1 },
                    { 48, 13, 1 },
                    { 49, 14, 1 },
                    { 50, 15, 1 },
                    { 51, 16, 1 },
                    { 52, 17, 1 },
                    { 53, 18, 1 },
                    { 54, 19, 1 },
                    { 55, 20, 1 },
                    { 56, 21, 1 },
                    { 57, 22, 1 },
                    { 58, 23, 1 },
                    { 59, 24, 1 },
                    { 60, 25, 1 },
                    { 61, 26, 1 },
                    { 62, 27, 1 },
                    { 63, 28, 1 },
                    { 64, 29, 1 },
                    { 65, 30, 1 },
                    { 66, 31, 1 },
                    { 67, 32, 1 },
                    { 68, 34, 1 },
                    { 69, 35, 1 },
                    { 70, 36, 1 },
                    { 71, 33, 6 },
                    { 72, 1, 6 },
                    { 73, 8, 6 },
                    { 74, 10, 6 },
                    { 75, 15, 6 },
                    { 76, 20, 6 },
                    { 77, 25, 6 },
                    { 78, 30, 6 },
                    { 79, 35, 6 },
                    { 80, 1, 2 },
                    { 81, 2, 2 },
                    { 82, 4, 2 },
                    { 83, 5, 2 },
                    { 84, 8, 2 },
                    { 85, 10, 2 },
                    { 86, 11, 2 },
                    { 87, 12, 2 },
                    { 88, 13, 2 },
                    { 89, 14, 2 },
                    { 90, 15, 2 },
                    { 91, 16, 2 },
                    { 92, 17, 2 },
                    { 93, 18, 2 },
                    { 94, 20, 2 },
                    { 95, 21, 2 },
                    { 96, 22, 2 },
                    { 97, 24, 2 },
                    { 98, 25, 2 },
                    { 99, 26, 2 },
                    { 100, 27, 2 },
                    { 101, 29, 2 },
                    { 102, 30, 2 },
                    { 103, 31, 2 },
                    { 104, 32, 2 },
                    { 105, 34, 2 },
                    { 106, 35, 2 },
                    { 107, 36, 2 },
                    { 108, 1, 3 },
                    { 109, 8, 3 },
                    { 110, 10, 3 },
                    { 111, 15, 3 },
                    { 112, 17, 3 },
                    { 113, 20, 3 },
                    { 114, 21, 3 },
                    { 115, 22, 3 },
                    { 116, 24, 3 },
                    { 117, 25, 3 },
                    { 118, 26, 3 },
                    { 119, 27, 3 },
                    { 120, 29, 3 },
                    { 121, 30, 3 },
                    { 122, 34, 3 },
                    { 123, 35, 3 },
                    { 124, 1, 4 },
                    { 125, 8, 4 },
                    { 126, 10, 4 },
                    { 127, 15, 4 },
                    { 128, 20, 4 },
                    { 129, 24, 4 },
                    { 130, 25, 4 },
                    { 131, 29, 4 },
                    { 132, 30, 4 },
                    { 133, 35, 4 },
                    { 134, 1, 5 },
                    { 135, 8, 5 },
                    { 136, 10, 5 },
                    { 137, 15, 5 },
                    { 138, 20, 5 },
                    { 139, 24, 5 },
                    { 140, 25, 5 },
                    { 141, 29, 5 },
                    { 142, 30, 5 },
                    { 143, 35, 5 },
                    { 144, 1, 8 },
                    { 145, 10, 8 },
                    { 146, 15, 8 },
                    { 147, 35, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_artifact_versions_artifact_id_version_number",
                table: "artifact_versions",
                columns: new[] { "artifact_id", "version_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_artifact_versions_created_by",
                table: "artifact_versions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_artifacts_phase_id",
                table: "artifacts",
                column: "phase_id");

            migrationBuilder.CreateIndex(
                name: "IX_artifacts_project_id",
                table: "artifacts",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_iterations_phase_id",
                table: "iterations",
                column: "phase_id");

            migrationBuilder.CreateIndex(
                name: "IX_microincrements_artifact_id",
                table: "microincrements",
                column: "artifact_id");

            migrationBuilder.CreateIndex(
                name: "IX_microincrements_author_id",
                table: "microincrements",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_microincrements_iteration_id",
                table: "microincrements",
                column: "iteration_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_phases_project_id",
                table: "phases",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_users_project_id",
                table: "project_users",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_users_role_id",
                table: "project_users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_users_user_id",
                table: "project_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_projects_code",
                table: "projects",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_created_by",
                table: "projects",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_role_id",
                table: "role_permissions",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "artifact_versions");

            migrationBuilder.DropTable(
                name: "microincrements");

            migrationBuilder.DropTable(
                name: "project_users");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "artifacts");

            migrationBuilder.DropTable(
                name: "iterations");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "phases");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
