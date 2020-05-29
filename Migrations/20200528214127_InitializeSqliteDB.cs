using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Discord_Bot_Tutorial.Migrations
{
    public partial class InitializeSqliteDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MutedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberID = table.Column<string>(nullable: true),
                    MutedReason = table.Column<string>(nullable: true),
                    MutedExpiration = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutedUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userID = table.Column<ulong>(nullable: false),
                    userName = table.Column<string>(nullable: true),
                    dps = table.Column<int>(nullable: false),
                    tank = table.Column<int>(nullable: false),
                    support = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MutedUsers");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
