using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetIdentity.Migrations
{
    public partial class AppUserDuplicatedFieldsRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomPhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CustomUserName",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomPhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomUserName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
