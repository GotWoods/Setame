using System;
using System.Collections.Generic;
using ConfigMan.Data.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigMan.Data.Migrations
{
    /// <inheritdoc />
    public partial class ToJsonB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Setting_Applications_ApplicationId",
                table: "Setting");

            migrationBuilder.DropIndex(
                name: "IX_Setting_ApplicationId",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Setting");

            migrationBuilder.AddColumn<List<Setting>>(
                name: "ApplicationDefaults",
                table: "Applications",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationDefaults",
                table: "Applications");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationId",
                table: "Setting",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Setting_ApplicationId",
                table: "Setting",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Setting_Applications_ApplicationId",
                table: "Setting",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id");
        }
    }
}
