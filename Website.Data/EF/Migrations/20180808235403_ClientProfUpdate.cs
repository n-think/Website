using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class ClientProfUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ClientProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "ClientProfiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatrName",
                table: "ClientProfiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ClientProfiles");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "ClientProfiles");

            migrationBuilder.DropColumn(
                name: "PatrName",
                table: "ClientProfiles");
        }
    }
}
