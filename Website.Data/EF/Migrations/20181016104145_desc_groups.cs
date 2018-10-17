using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class desc_groups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DescriptionGroups_DescriptionGroups_ParentId",
                table: "DescriptionGroups");

            migrationBuilder.DropIndex(
                name: "IX_DescriptionGroups_ParentId",
                table: "DescriptionGroups");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "DescriptionGroups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "DescriptionGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DescriptionGroups_ParentId",
                table: "DescriptionGroups",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DescriptionGroups_DescriptionGroups_ParentId",
                table: "DescriptionGroups",
                column: "ParentId",
                principalTable: "DescriptionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
