using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class image_add_path_format : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "ProductImages",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbPath",
                table: "ProductImages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Format",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ThumbPath",
                table: "ProductImages");
        }
    }
}
