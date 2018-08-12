using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class Prod_Profile_upd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "ClientProfiles");

            migrationBuilder.AddColumn<int>(
                name: "Code",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ClientProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ClientProfiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "ClientProfiles");

            migrationBuilder.DropColumn(
                name: "City",
                table: "ClientProfiles");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "ClientProfiles",
                nullable: false,
                defaultValue: 0);
        }
    }
}
