using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class renamed_added_created : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Added",
                table: "Products",
                newName: "Created");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Products",
                newName: "Added");
        }
    }
}
