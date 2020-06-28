using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diet.Api.Data.Migrations
{
    public partial class CaloriesTypeUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("a2814c0f-28b5-4815-9628-bf9bff6dd8c5"));

            migrationBuilder.AlterColumn<decimal>(
                name: "Calories",
                table: "Meal",
                type: "decimal(6,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("2d7d4516-5132-4652-b3ef-f0ef3af2c1fb"), "admin@domain.com", "AAAAAAEAAAAQMVftd8Xr4QQTfvAhevT7nfMsCKmeqleYlArl9Me3tp3UOrP0xWFK0A/5o7zEd5CJ", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("2d7d4516-5132-4652-b3ef-f0ef3af2c1fb"));

            migrationBuilder.AlterColumn<decimal>(
                name: "Calories",
                table: "Meal",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("a2814c0f-28b5-4815-9628-bf9bff6dd8c5"), "admin@domain.com", "AAAAAAEAAAAQC6Dkc40/h3pN9W4sbAAbnVNS7M9CMpBtnO8KPrGpHd2KL/dpqC38aA8a7N0BvB8+", "Admin" });
        }
    }
}
