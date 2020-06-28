using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diet.Api.Data.Migrations
{
    public partial class CreateAccountSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("be1f05cc-d57d-461c-9e33-8a2ec5d8ff34"));

            migrationBuilder.CreateTable(
                name: "AccountSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TargetCalories = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountSetting_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("172b12e2-8ea5-4011-8960-ee71c0fe5940"), "admin@domain.com", "AAAAAAEAAAAQHfgmjF/Kcz5Hpn5d2peYbyAaEMC7PA49pHkx4M46XH8K7XUvt34EAsJzYwbo9kZt", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSetting_AccountId",
                table: "AccountSetting",
                column: "AccountId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSetting");

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("172b12e2-8ea5-4011-8960-ee71c0fe5940"));

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("be1f05cc-d57d-461c-9e33-8a2ec5d8ff34"), "admin@domain.com", "AAAAAAEAAAAQRzm9HfyNeOEKdF23Xtg75zW14RTcB6v3Use/PM2ZPKaDvF+ZNk4pdnHpN0QmJde6", "Admin" });
        }
    }
}
