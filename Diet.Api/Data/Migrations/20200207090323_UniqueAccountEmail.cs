using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diet.Api.Data.Migrations
{
    public partial class UniqueAccountEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("2d7d4516-5132-4652-b3ef-f0ef3af2c1fb"));

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("be1f05cc-d57d-461c-9e33-8a2ec5d8ff34"), "admin@domain.com", "AAAAAAEAAAAQRzm9HfyNeOEKdF23Xtg75zW14RTcB6v3Use/PM2ZPKaDvF+ZNk4pdnHpN0QmJde6", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Email",
                table: "Account",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Account_Email",
                table: "Account");

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("be1f05cc-d57d-461c-9e33-8a2ec5d8ff34"));

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("2d7d4516-5132-4652-b3ef-f0ef3af2c1fb"), "admin@domain.com", "AAAAAAEAAAAQMVftd8Xr4QQTfvAhevT7nfMsCKmeqleYlArl9Me3tp3UOrP0xWFK0A/5o7zEd5CJ", "Admin" });
        }
    }
}
