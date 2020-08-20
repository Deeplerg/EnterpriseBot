using Microsoft.EntityFrameworkCore.Migrations;

namespace EnterpriseBot.Api.Migrations
{
    public partial class PlayerWithoutPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanChangeNameAfterRegistrationViaSocialNetwork",
                table: "Players",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RegisteredWithSocialNetworkCredentials",
                table: "Players",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanChangeNameAfterRegistrationViaSocialNetwork",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "RegisteredWithSocialNetworkCredentials",
                table: "Players");
        }
    }
}
