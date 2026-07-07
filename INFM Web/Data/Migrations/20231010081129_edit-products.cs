using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CS8981

namespace INFM_Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class editproducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_Category_Id1",
                table: "Product");


            migrationBuilder.AlterColumn<int>(
                name: "Category_Id1",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_Category_Id1",
                table: "Product",
                column: "Category_Id1",
                principalTable: "Category",
                principalColumn: "Category_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_Category_Id1",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "Category_Id1",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_Category_Id1",
                table: "Product",
                column: "Category_Id1",
                principalTable: "Category",
                principalColumn: "Category_Id");
        }
    }
}
