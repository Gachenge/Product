using Microsoft.EntityFrameworkCore.Migrations;

namespace Abno.Data.Migrations
{
    public partial class addusersColors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Primary",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Secondary",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Primary",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Secondary",
                table: "AspNetUsers");
        }
    }
}
