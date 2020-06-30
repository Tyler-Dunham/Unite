using Microsoft.EntityFrameworkCore.Migrations;

namespace Discord_Bot_Tutorial.Migrations
{
    public partial class AddedTeamMoving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Team",
                table: "playerQueue",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Team",
                table: "playerQueue");
        }
    }
}
