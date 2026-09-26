using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VoxDB.Entities.Migrations
{
    /// <inheritdoc />
    public partial class IsolatedBrowserSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "BrowserSessionId",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BrowserSessionId",
                table: "ChatSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BrowserSessionId",
                table: "ChatMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BrowserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrowserSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BrowserSessionId",
                table: "Employees",
                column: "BrowserSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_BrowserSessionId",
                table: "ChatSessions",
                column: "BrowserSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_BrowserSessionId",
                table: "ChatMessages",
                column: "BrowserSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_BrowserSessions_BrowserSessionId",
                table: "ChatMessages",
                column: "BrowserSessionId",
                principalTable: "BrowserSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_BrowserSessions_BrowserSessionId",
                table: "ChatSessions",
                column: "BrowserSessionId",
                principalTable: "BrowserSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_BrowserSessions_BrowserSessionId",
                table: "Employees",
                column: "BrowserSessionId",
                principalTable: "BrowserSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_BrowserSessions_BrowserSessionId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_BrowserSessions_BrowserSessionId",
                table: "ChatSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_BrowserSessions_BrowserSessionId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "BrowserSessions");

            migrationBuilder.DropIndex(
                name: "IX_Employees_BrowserSessionId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_ChatSessions_BrowserSessionId",
                table: "ChatSessions");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_BrowserSessionId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "BrowserSessionId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BrowserSessionId",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "BrowserSessionId",
                table: "ChatMessages");

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "FullName", "Position" },
                values: new object[,]
                {
                    { 1, "Ivan Ivanov", "Engineer" },
                    { 2, "Alex Baena", "Analyst" }
                });
        }
    }
}
