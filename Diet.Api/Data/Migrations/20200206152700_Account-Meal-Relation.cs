using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Diet.Api.Data.Migrations
{
    public partial class AccountMealRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Meal",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Meal_AccountId",
                table: "Meal",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meal_Account_AccountId",
                table: "Meal",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meal_Account_AccountId",
                table: "Meal");

            migrationBuilder.DropIndex(
                name: "IX_Meal_AccountId",
                table: "Meal");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Meal");
        }
    }
}
