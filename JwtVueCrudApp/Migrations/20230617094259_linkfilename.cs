using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtVueCrudApp.Migrations
{
    /// <inheritdoc />
    public partial class linkfilename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFiles_Products_ProductId",
                table: "ProductFiles");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductFiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "LInkFileName",
                table: "ProductFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFiles_Products_ProductId",
                table: "ProductFiles",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFiles_Products_ProductId",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "LInkFileName",
                table: "ProductFiles");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductFiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFiles_Products_ProductId",
                table: "ProductFiles",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
