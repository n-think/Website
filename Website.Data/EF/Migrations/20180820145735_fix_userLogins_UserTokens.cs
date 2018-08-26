using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class fix_userLogins_UserTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId1",
                table: "UserLogins");

            migrationBuilder.DropIndex(
                name: "IX_UserLogins_UserId1",
                table: "UserLogins");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserLogins");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserLogins",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId1",
                table: "UserLogins",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId1",
                table: "UserLogins",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
