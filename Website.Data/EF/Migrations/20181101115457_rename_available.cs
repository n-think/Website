using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class rename_available : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Enabled",
                table: "Products",
                newName: "Available");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Available",
                table: "Products",
                newName: "Enabled");
        }
    }
}
