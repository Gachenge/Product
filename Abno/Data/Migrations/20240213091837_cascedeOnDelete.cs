using Microsoft.EntityFrameworkCore.Migrations;

namespace Abno.Data.Migrations
{
    public partial class cascedeOnDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_AspNetUsers_UserId",
                table: "UserProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_AspNetUsers_UserId",
                table: "UserProducts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_AspNetUsers_UserId",
                table: "UserProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_AspNetUsers_UserId",
                table: "UserProducts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
