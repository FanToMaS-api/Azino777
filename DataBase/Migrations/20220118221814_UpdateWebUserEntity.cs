using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBase.Migrations
{
    public partial class UpdateWebUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_web_users_username",
                table: "web_users");

            migrationBuilder.CreateIndex(
                name: "IX_web_users_username",
                table: "web_users",
                column: "username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_web_users_username",
                table: "web_users");

            migrationBuilder.CreateIndex(
                name: "IX_web_users_username",
                table: "web_users",
                column: "username",
                unique: true);
        }
    }
}
