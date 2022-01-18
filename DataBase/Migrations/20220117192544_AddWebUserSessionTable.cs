using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataBase.Migrations
{
    public partial class AddWebUserSessionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_web_users_id",
                table: "web_users");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "web_users",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "web_users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "web_user_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    expired = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_web_user_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_web_user_sessions_web_users_user_id",
                        column: x => x.user_id,
                        principalTable: "web_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_web_users_id",
                table: "web_users",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_state_id",
                table: "users_state",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_id",
                table: "users",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roulette_history_id",
                table: "roulette_history",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_referral_links_id",
                table: "referral_links",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_blackjack_history_id",
                table: "blackjack_history",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_web_user_sessions_expired",
                table: "web_user_sessions",
                column: "expired");

            migrationBuilder.CreateIndex(
                name: "IX_web_user_sessions_id",
                table: "web_user_sessions",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_web_user_sessions_status",
                table: "web_user_sessions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_web_user_sessions_user_id",
                table: "web_user_sessions",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "web_user_sessions");

            migrationBuilder.DropIndex(
                name: "IX_web_users_id",
                table: "web_users");

            migrationBuilder.DropIndex(
                name: "IX_users_state_id",
                table: "users_state");

            migrationBuilder.DropIndex(
                name: "IX_users_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_roulette_history_id",
                table: "roulette_history");

            migrationBuilder.DropIndex(
                name: "IX_referral_links_id",
                table: "referral_links");

            migrationBuilder.DropIndex(
                name: "IX_blackjack_history_id",
                table: "blackjack_history");

            migrationBuilder.AlterColumn<int>(
                name: "role",
                table: "web_users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "web_users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_web_users_id",
                table: "web_users",
                column: "id");
        }
    }
}
