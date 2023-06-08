using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtVueCrudApp.Migrations
{
    /// <inheritdoc />
    public partial class reply_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Products_ProductId",
                table: "Replies");

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "Replies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_ProductId1",
                table: "Replies",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Products_ProductId",
                table: "Replies",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Products_ProductId1",
                table: "Replies",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Products_ProductId",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Products_ProductId1",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Replies_ProductId1",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "Replies");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Products_ProductId",
                table: "Replies",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
