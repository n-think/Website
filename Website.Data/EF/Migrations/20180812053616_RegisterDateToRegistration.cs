using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Data.EF.Migrations
{
    public partial class RegisterDateToRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                table: "ClientProfiles",
                newName: "RegistrationDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "ClientProfiles",
                newName: "RegisterDate");
        }
    }
}
