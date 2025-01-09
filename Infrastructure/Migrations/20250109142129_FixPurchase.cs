using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Products_ProductId1",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_ProductId1",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "Purchases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ProductId1",
                table: "Purchases",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Products_ProductId1",
                table: "Purchases",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}


