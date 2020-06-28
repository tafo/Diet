using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diet.Api.Data.Migrations
{
    public partial class AddCalorieStatusToMeal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("a9dad2fa-8eaf-4e3e-b34a-9fa7eb41e451"));

            migrationBuilder.AddColumn<bool>(
                name: "CalorieStatus",
                table: "Meal",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("96d63772-d7dc-4dcc-87e1-e1aa0a58ddef"), "admin@domain.com", "AAAAAAEAAAAQWjxUNna4xiVO224dSleHlCOddJexHidWfVZTg5P8aBV43AkiirmQSufUhuXHTi24", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("96d63772-d7dc-4dcc-87e1-e1aa0a58ddef"));

            migrationBuilder.DropColumn(
                name: "CalorieStatus",
                table: "Meal");

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("a9dad2fa-8eaf-4e3e-b34a-9fa7eb41e451"), "admin@domain.com", "AAAAAAEAAAAQ6tGqHudlSLvsPrnl50Ig8hwsZxcAhaNtqMOBmh3TCayTiSmmiUP/InHX9AEcTXUj", "Admin" });
        }
    }
}
