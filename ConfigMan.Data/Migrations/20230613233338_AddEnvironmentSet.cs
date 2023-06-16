using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigMan.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnvironmentSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnvironmentSet",
                table: "Applications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnvironmentSet",
                table: "Applications");
        }
    }
}
