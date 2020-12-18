using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplySocial.Server.Data.Identity.Migrations
{
    public partial class Ignoring_Profile_Picture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "Users",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
