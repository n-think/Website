using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class images_unique_names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ThumbName",
                table: "ProductImages",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductImages",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_Name",
                table: "ProductImages",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ThumbName",
                table: "ProductImages",
                column: "ThumbName",
                unique: true,
                filter: "[ThumbName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductImages_Name",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ThumbName",
                table: "ProductImages");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbName",
                table: "ProductImages",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductImages",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
