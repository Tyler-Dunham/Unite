using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Discord_Bot_Tutorial.Migrations
{
    public partial class RemoveBansMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bans");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    banReason = table.Column<string>(type: "TEXT", nullable: true),
                    banTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    unbanTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    userID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    userName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                });
        }
    }
}
