using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Abno.Data.Migrations
{
    public partial class addCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserProducts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserProducts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserProducts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Product");
        }
    }
}
