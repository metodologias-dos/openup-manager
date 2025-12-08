using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenUpMan.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToIterations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "iterations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "iterations");
        }
    }
}
