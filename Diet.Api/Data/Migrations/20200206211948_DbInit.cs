using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diet.Api.Data.Migrations
{
    public partial class DbInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("a2814c0f-28b5-4815-9628-bf9bff6dd8c5"), "admin@domain.com", "AAAAAAEAAAAQC6Dkc40/h3pN9W4sbAAbnVNS7M9CMpBtnO8KPrGpHd2KL/dpqC38aA8a7N0BvB8+", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("a2814c0f-28b5-4815-9628-bf9bff6dd8c5"));
        }
    }
}
