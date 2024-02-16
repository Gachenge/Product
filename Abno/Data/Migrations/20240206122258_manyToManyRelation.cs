using Microsoft.EntityFrameworkCore.Migrations;

namespace Abno.Data.Migrations
{
    public partial class manyToManyRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProduct_Product_ProductId",
                table: "UserProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProduct_AspNetUsers_UserId",
                table: "UserProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProduct",
                table: "UserProduct");

            migrationBuilder.RenameTable(
                name: "UserProduct",
                newName: "UserProducts");

            migrationBuilder.RenameIndex(
                name: "IX_UserProduct_UserId",
                table: "UserProducts",
                newName: "IX_UserProducts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProduct_ProductId",
                table: "UserProducts",
                newName: "IX_UserProducts_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProducts",
                table: "UserProducts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_Product_ProductId",
                table: "UserProducts",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_AspNetUsers_UserId",
                table: "UserProducts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_Product_ProductId",
                table: "UserProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_AspNetUsers_UserId",
                table: "UserProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProducts",
                table: "UserProducts");

            migrationBuilder.RenameTable(
                name: "UserProducts",
                newName: "UserProduct");

            migrationBuilder.RenameIndex(
                name: "IX_UserProducts_UserId",
                table: "UserProduct",
                newName: "IX_UserProduct_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProducts_ProductId",
                table: "UserProduct",
                newName: "IX_UserProduct_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProduct",
                table: "UserProduct",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProduct_Product_ProductId",
                table: "UserProduct",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProduct_AspNetUsers_UserId",
                table: "UserProduct",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
