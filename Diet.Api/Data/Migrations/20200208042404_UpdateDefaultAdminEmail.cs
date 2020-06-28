using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diet.Api.Data.Migrations
{
    public partial class UpdateDefaultAdminEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("96d63772-d7dc-4dcc-87e1-e1aa0a58ddef"));

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("f825d901-4576-4375-8e90-24e5aecc70f2"), "admin@string.com", "AAAAAAEAAAAQrkn/4hNB5cdHHyht85OZTbT9gBK8G8bb3agj6KMgdvNRcTi/IKhhZASyVAZG0R+8", "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "Id",
                keyValue: new Guid("f825d901-4576-4375-8e90-24e5aecc70f2"));

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "Id", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("96d63772-d7dc-4dcc-87e1-e1aa0a58ddef"), "admin@domain.com", "AAAAAAEAAAAQWjxUNna4xiVO224dSleHlCOddJexHidWfVZTg5P8aBV43AkiirmQSufUhuXHTi24", "Admin" });
        }
    }
}
