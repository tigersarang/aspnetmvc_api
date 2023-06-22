using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtVueCrudApp.Migrations
{
    /// <inheritdoc />
    public partial class filesCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFiles_Products_ProductId",
                table: "ProductFiles");

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "ProductFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFiles_ProductId1",
                table: "ProductFiles",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFiles_Products_ProductId",
                table: "ProductFiles",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFiles_Products_ProductId1",
                table: "ProductFiles",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFiles_Products_ProductId",
                table: "ProductFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFiles_Products_ProductId1",
                table: "ProductFiles");

            migrationBuilder.DropIndex(
                name: "IX_ProductFiles_ProductId1",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductFiles");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFiles_Products_ProductId",
                table: "ProductFiles",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
