using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ZDC.Server.Migrations
{
    public partial class ChangeNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Users_UserId",
                table: "News");

            migrationBuilder.DropTable(
                name: "DebugLogs");

            migrationBuilder.DropIndex(
                name: "IX_News_UserId",
                table: "News");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "News",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "News");

            migrationBuilder.CreateTable(
                name: "DebugLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cid = table.Column<string>(type: "text", nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: false),
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Route = table.Column<string>(type: "text", nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebugLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_News_UserId",
                table: "News",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Users_UserId",
                table: "News",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
