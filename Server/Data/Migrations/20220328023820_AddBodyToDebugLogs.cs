using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZDC.Server.Migrations
{
    public partial class AddBodyToDebugLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "DebugLogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "DebugLogs");
        }
    }
}
