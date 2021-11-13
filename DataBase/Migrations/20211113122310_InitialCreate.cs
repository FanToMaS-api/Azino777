using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataBase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blackjack_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    dialer_scope = table.Column<int>(type: "integer", nullable: false),
                    user_scope = table.Column<int>(type: "integer", nullable: false),
                    bid = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blackjack_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roulette_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    coin = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roulette_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nickname = table.Column<string>(type: "text", nullable: true),
                    lastname = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    last_action = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users_state",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    balance = table.Column<double>(type: "double precision", nullable: false),
                    state_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_state", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blackjack_history_bid",
                table: "blackjack_history",
                column: "bid");

            migrationBuilder.CreateIndex(
                name: "IX_blackjack_history_dialer_scope",
                table: "blackjack_history",
                column: "dialer_scope");

            migrationBuilder.CreateIndex(
                name: "IX_blackjack_history_state",
                table: "blackjack_history",
                column: "state");

            migrationBuilder.CreateIndex(
                name: "IX_blackjack_history_user_id",
                table: "blackjack_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_blackjack_history_user_scope",
                table: "blackjack_history",
                column: "user_scope");

            migrationBuilder.CreateIndex(
                name: "IX_roulette_history_coin",
                table: "roulette_history",
                column: "coin");

            migrationBuilder.CreateIndex(
                name: "IX_roulette_history_state",
                table: "roulette_history",
                column: "state");

            migrationBuilder.CreateIndex(
                name: "IX_roulette_history_user_id",
                table: "roulette_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_firstname",
                table: "users",
                column: "firstname");

            migrationBuilder.CreateIndex(
                name: "IX_users_last_action",
                table: "users",
                column: "last_action");

            migrationBuilder.CreateIndex(
                name: "IX_users_lastname",
                table: "users",
                column: "lastname");

            migrationBuilder.CreateIndex(
                name: "IX_users_nickname",
                table: "users",
                column: "nickname");

            migrationBuilder.CreateIndex(
                name: "IX_users_phone_number",
                table: "users",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_state_balance",
                table: "users_state",
                column: "balance");

            migrationBuilder.CreateIndex(
                name: "IX_users_state_state_type",
                table: "users_state",
                column: "state_type");

            migrationBuilder.CreateIndex(
                name: "IX_users_state_user_id",
                table: "users_state",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blackjack_history");

            migrationBuilder.DropTable(
                name: "roulette_history");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "users_state");
        }
    }
}
