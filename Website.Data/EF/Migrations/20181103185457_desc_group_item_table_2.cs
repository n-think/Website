using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class desc_group_item_table_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DescriptionGroupItemId",
                table: "Descriptions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Descriptions_DescriptionGroupItemId",
                table: "Descriptions",
                column: "DescriptionGroupItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionGroupItems_DescriptionGroupId",
                table: "DescriptionGroupItems",
                column: "DescriptionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionGroupItems_DescriptionGroups",
                table: "DescriptionGroupItems",
                column: "DescriptionGroupId",
                principalTable: "DescriptionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Descriptions_DescriptionGroupItems_DescriptionGroupItemId",
                table: "Descriptions",
                column: "DescriptionGroupItemId",
                principalTable: "DescriptionGroupItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionGroupItems_DescriptionGroups",
                table: "DescriptionGroupItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Descriptions_DescriptionGroupItems_DescriptionGroupItemId",
                table: "Descriptions");

            migrationBuilder.DropIndex(
                name: "IX_Descriptions_DescriptionGroupItemId",
                table: "Descriptions");

            migrationBuilder.DropIndex(
                name: "IX_DescriptionGroupItems_DescriptionGroupId",
                table: "DescriptionGroupItems");

            migrationBuilder.DropColumn(
                name: "DescriptionGroupItemId",
                table: "Descriptions");
        }
    }
}
