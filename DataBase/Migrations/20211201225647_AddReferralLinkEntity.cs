using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataBase.Migrations
{
    public partial class AddReferralLinkEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ban_reason",
                table: "users_state",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "referral_link",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "referral_links",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    referral_link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referral_links", x => x.id);
                    table.ForeignKey(
                        name: "FK_referral_links_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_state_ban_reason",
                table: "users_state",
                column: "ban_reason");

            migrationBuilder.CreateIndex(
                name: "IX_users_referral_link",
                table: "users",
                column: "referral_link");

            migrationBuilder.CreateIndex(
                name: "IX_referral_links_referral_link",
                table: "referral_links",
                column: "referral_link",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_referral_links_user_id",
                table: "referral_links",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "referral_links");

            migrationBuilder.DropIndex(
                name: "IX_users_state_ban_reason",
                table: "users_state");

            migrationBuilder.DropIndex(
                name: "IX_users_referral_link",
                table: "users");

            migrationBuilder.DropColumn(
                name: "ban_reason",
                table: "users_state");

            migrationBuilder.DropColumn(
                name: "referral_link",
                table: "users");
        }
    }
}
