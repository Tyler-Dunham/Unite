using Microsoft.EntityFrameworkCore.Migrations;

namespace Discord_Bot_Tutorial.Migrations
{
    public partial class BanReasonMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "banReason",
                table: "Bans",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "banReason",
                table: "Bans");
        }
    }
}
