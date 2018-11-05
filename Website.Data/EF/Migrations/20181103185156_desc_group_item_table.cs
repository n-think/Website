using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class desc_group_item_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Descriptions_DescriptionGroups",
                table: "Descriptions");

            migrationBuilder.DropIndex(
                name: "IX_Descriptions_DescriptionGroupId",
                table: "Descriptions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Descriptions_DescriptionGroupId",
                table: "Descriptions",
                column: "DescriptionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Descriptions_DescriptionGroups",
                table: "Descriptions",
                column: "DescriptionGroupId",
                principalTable: "DescriptionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
