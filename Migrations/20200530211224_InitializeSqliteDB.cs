using Microsoft.EntityFrameworkCore.Migrations;

namespace Discord_Bot_Tutorial.Migrations
{
    public partial class InitializeSqliteDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "playerQueue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userID = table.Column<ulong>(nullable: false),
                    userName = table.Column<string>(nullable: true),
                    role = table.Column<string>(nullable: true),
                    queueSr = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playerQueue", x => x.Id);
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
                    support = table.Column<int>(nullable: false),
                    queue = table.Column<bool>(nullable: false),
                    role = table.Column<string>(nullable: true),
                    queueSr = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "playerQueue");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
