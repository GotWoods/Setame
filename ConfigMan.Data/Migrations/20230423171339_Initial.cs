using System;
using System.Collections.Generic;
using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigMan.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ApplicationDefaults = table.Column<List<Setting>>(type: "jsonb", nullable: true),
                    EnvironmentSettings = table.Column<Dictionary<string, List<Setting>>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentGroups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    EnvironmentSettings = table.Column<Dictionary<string, List<Setting>>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentGroups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Environments",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    DeploymentEnvironments = table.Column<List<DeploymentEnvironment>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Environments", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VariableGroups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Settings = table.Column<List<Setting>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariableGroups", x => x.Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "EnvironmentGroups");

            migrationBuilder.DropTable(
                name: "Environments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VariableGroups");
        }
    }
}
