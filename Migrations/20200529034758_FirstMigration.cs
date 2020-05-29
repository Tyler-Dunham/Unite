using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Discord_Bot_Tutorial.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MutedUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MutedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberID = table.Column<string>(type: "TEXT", nullable: true),
                    MutedExpiration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MutedReason = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutedUsers", x => x.Id);
                });
        }
    }
}
