using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_AspNetUsers_UserId",
                table: "Purchases");

            migrationBuilder.AddColumn<Guid>(
                name: "SellerId",
                table: "Purchases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SellerId",
                table: "Purchases",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_AspNetUsers_SellerId",
                table: "Purchases",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_AspNetUsers_UserId",
                table: "Purchases",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_AspNetUsers_SellerId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_AspNetUsers_UserId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_SellerId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Purchases");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_AspNetUsers_UserId",
                table: "Purchases",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
