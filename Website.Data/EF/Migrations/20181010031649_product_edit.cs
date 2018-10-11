using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class product_edit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbImage",
                table: "Products");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Added",
                table: "Products",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Changed",
                table: "Products",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Added",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Changed",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Products");

            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbImage",
                table: "Products",
                nullable: true);
        }
    }
}
