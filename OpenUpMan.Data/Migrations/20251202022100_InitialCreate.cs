using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenUpMan.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
