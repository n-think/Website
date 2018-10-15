using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class images_filenames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThumbPath",
                table: "ProductImages",
                newName: "ThumbName");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductImages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductImages");

            migrationBuilder.RenameColumn(
                name: "ThumbName",
                table: "ProductImages",
                newName: "ThumbPath");
        }
    }
}
