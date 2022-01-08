using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataBase.Migrations
{
    public partial class AddIdentityDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blackjack_history_users_user_id",
                table: "blackjack_history");

            migrationBuilder.DropForeignKey(
                name: "FK_referral_links_users_user_id",
                table: "referral_links");

            migrationBuilder.DropForeignKey(
                name: "FK_roulette_history_users_user_id",
                table: "roulette_history");

            migrationBuilder.DropTable(
                name: "users_state");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "roulette_history",
                newName: "bot_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_roulette_history_user_id",
                table: "roulette_history",
                newName: "IX_roulette_history_bot_user_id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "referral_links",
                newName: "bot_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_referral_links_user_id",
                table: "referral_links",
                newName: "IX_referral_links_bot_user_id");

            migrationBuilder.RenameColumn(
                name: "user_scope",
                table: "blackjack_history",
                newName: "bot_user_scope");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "blackjack_history",
                newName: "bot_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_blackjack_history_user_scope",
                table: "blackjack_history",
                newName: "IX_blackjack_history_bot_user_scope");

            migrationBuilder.RenameIndex(
                name: "IX_blackjack_history_user_id",
                table: "blackjack_history",
                newName: "IX_blackjack_history_bot_user_id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bot_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: true),
                    lastname = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(type: "text", nullable: true),
                    last_action = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    referral_link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bot_users_state",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    balance = table.Column<double>(type: "double precision", nullable: false),
                    state_type = table.Column<string>(type: "text", nullable: false),
                    ban_reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bot_users_state", x => x.id);
                    table.ForeignKey(
                        name: "FK_bot_users_state_bot_users_user_id",
                        column: x => x.user_id,
                        principalTable: "bot_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_role_id",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_user_id",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_user_id",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_role_id",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_chat_id",
                table: "bot_users",
                column: "chat_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_firstname",
                table: "bot_users",
                column: "firstname");

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_last_action",
                table: "bot_users",
                column: "last_action");

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_lastname",
                table: "bot_users",
                column: "lastname");

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_nickname",
                table: "bot_users",
                column: "nickname");

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_referral_link",
                table: "bot_users",
                column: "referral_link");

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_telegram_id",
                table: "bot_users",
                column: "telegram_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_state_balance",
                table: "bot_users_state",
                column: "balance");

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_state_ban_reason",
                table: "bot_users_state",
                column: "ban_reason");

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_state_state_type",
                table: "bot_users_state",
                column: "state_type");

            migrationBuilder.CreateIndex(
                name: "IX_bot_users_state_user_id",
                table: "bot_users_state",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_blackjack_history_bot_users_bot_user_id",
                table: "blackjack_history",
                column: "bot_user_id",
                principalTable: "bot_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_referral_links_bot_users_bot_user_id",
                table: "referral_links",
                column: "bot_user_id",
                principalTable: "bot_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_roulette_history_bot_users_bot_user_id",
                table: "roulette_history",
                column: "bot_user_id",
                principalTable: "bot_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blackjack_history_bot_users_bot_user_id",
                table: "blackjack_history");

            migrationBuilder.DropForeignKey(
                name: "FK_referral_links_bot_users_bot_user_id",
                table: "referral_links");

            migrationBuilder.DropForeignKey(
                name: "FK_roulette_history_bot_users_bot_user_id",
                table: "roulette_history");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "bot_users_state");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "bot_users");

            migrationBuilder.RenameColumn(
                name: "bot_user_id",
                table: "roulette_history",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_roulette_history_bot_user_id",
                table: "roulette_history",
                newName: "IX_roulette_history_user_id");

            migrationBuilder.RenameColumn(
                name: "bot_user_id",
                table: "referral_links",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_referral_links_bot_user_id",
                table: "referral_links",
                newName: "IX_referral_links_user_id");

            migrationBuilder.RenameColumn(
                name: "bot_user_scope",
                table: "blackjack_history",
                newName: "user_scope");

            migrationBuilder.RenameColumn(
                name: "bot_user_id",
                table: "blackjack_history",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_blackjack_history_bot_user_scope",
                table: "blackjack_history",
                newName: "IX_blackjack_history_user_scope");

            migrationBuilder.RenameIndex(
                name: "IX_blackjack_history_bot_user_id",
                table: "blackjack_history",
                newName: "IX_blackjack_history_user_id");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    firstname = table.Column<string>(type: "text", nullable: true),
                    last_action = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    lastname = table.Column<string>(type: "text", nullable: true),
                    nickname = table.Column<string>(type: "text", nullable: true),
                    referral_link = table.Column<string>(type: "text", nullable: true),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false)
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
                    balance = table.Column<double>(type: "double precision", nullable: false),
                    ban_reason = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    state_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_state", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_state_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_chat_id",
                table: "users",
                column: "chat_id",
                unique: true);

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
                name: "IX_users_referral_link",
                table: "users",
                column: "referral_link");

            migrationBuilder.CreateIndex(
                name: "IX_users_telegram_id",
                table: "users",
                column: "telegram_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_state_balance",
                table: "users_state",
                column: "balance");

            migrationBuilder.CreateIndex(
                name: "IX_users_state_ban_reason",
                table: "users_state",
                column: "ban_reason");

            migrationBuilder.CreateIndex(
                name: "IX_users_state_state_type",
                table: "users_state",
                column: "state_type");

            migrationBuilder.CreateIndex(
                name: "IX_users_state_user_id",
                table: "users_state",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_blackjack_history_users_user_id",
                table: "blackjack_history",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_referral_links_users_user_id",
                table: "referral_links",
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
        }
    }
}
