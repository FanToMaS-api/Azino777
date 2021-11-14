using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBase.Migrations
{
    public partial class AddedRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "chat_id",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_users_chat_id",
                table: "users",
                column: "chat_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_blackjack_history_users_user_id",
                table: "blackjack_history",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_roulette_history_users_user_id",
                table: "roulette_history",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_state_users_user_id",
                table: "users_state",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blackjack_history_users_user_id",
                table: "blackjack_history");

            migrationBuilder.DropForeignKey(
                name: "FK_roulette_history_users_user_id",
                table: "roulette_history");

            migrationBuilder.DropForeignKey(
                name: "FK_users_state_users_user_id",
                table: "users_state");

            migrationBuilder.DropIndex(
                name: "IX_users_chat_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "chat_id",
                table: "users");
        }
    }
}
