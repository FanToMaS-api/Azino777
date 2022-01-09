using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBase.Migrations
{
    public partial class AddSpamLimitsToBotUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "warning_number",
                table: "bot_users_state",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_state_warning_number",
                table: "bot_users_state",
                column: "warning_number");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_bot_users_state_warning_number",
                table: "bot_users_state");

            migrationBuilder.DropColumn(
                name: "warning_number",
                table: "bot_users_state");
        }
    }
}
