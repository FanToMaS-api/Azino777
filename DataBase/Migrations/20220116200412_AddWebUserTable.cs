using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataBase.Migrations
{
    public partial class AddWebUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "web_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_web_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_web_users_created",
                table: "web_users",
                column: "created");

            migrationBuilder.CreateIndex(
                name: "IX_web_users_id",
                table: "web_users",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_web_users_password",
                table: "web_users",
                column: "password");

            migrationBuilder.CreateIndex(
                name: "IX_web_users_role",
                table: "web_users",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "IX_web_users_updated",
                table: "web_users",
                column: "updated");

            migrationBuilder.CreateIndex(
                name: "IX_web_users_username",
                table: "web_users",
                column: "username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "web_users");
        }
    }
}
