using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class fix_UserClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_Users_UserId1",
                table: "UserClaims");

            migrationBuilder.DropIndex(
                name: "IX_UserClaims_UserId1",
                table: "UserClaims");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserClaims");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserClaims",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId1",
                table: "UserClaims",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_Users_UserId1",
                table: "UserClaims",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
