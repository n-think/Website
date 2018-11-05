using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class desc_group_item_table_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Descriptions_DescriptionGroupItems_DescriptionGroupItemId",
                table: "Descriptions");

            migrationBuilder.DropColumn(
                name: "DescriptionGroupId",
                table: "Descriptions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Descriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Descriptions_DescriptionGroupItems",
                table: "Descriptions",
                column: "DescriptionGroupItemId",
                principalTable: "DescriptionGroupItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Descriptions_DescriptionGroupItems",
                table: "Descriptions");

            migrationBuilder.AddColumn<int>(
                name: "DescriptionGroupId",
                table: "Descriptions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Descriptions",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Descriptions_DescriptionGroupItems_DescriptionGroupItemId",
                table: "Descriptions",
                column: "DescriptionGroupItemId",
                principalTable: "DescriptionGroupItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
