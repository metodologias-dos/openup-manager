using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenUpMan.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCompletionPercentageFromIterations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "completion_percentage",
                table: "iterations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "completion_percentage",
                table: "iterations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
