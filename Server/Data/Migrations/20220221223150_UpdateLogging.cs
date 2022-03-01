using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZDC.Server.Migrations
{
    public partial class UpdateLogging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "WebsiteLogs",
                newName: "Cid");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "WebsiteLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cid",
                table: "DebugLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Route",
                table: "DebugLogs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "WebsiteLogs");

            migrationBuilder.DropColumn(
                name: "Cid",
                table: "DebugLogs");

            migrationBuilder.DropColumn(
                name: "Route",
                table: "DebugLogs");

            migrationBuilder.RenameColumn(
                name: "Cid",
                table: "WebsiteLogs",
                newName: "Message");
        }
    }
}
