using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZDC.Server.Migrations
{
    public partial class UpdateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hours_Users_UserId",
                table: "Hours");

            migrationBuilder.DropIndex(
                name: "IX_Hours_UserId",
                table: "Hours");

            migrationBuilder.DropColumn(
                name: "SixMonthHours",
                table: "Warnings");

            migrationBuilder.AddColumn<int>(
                name: "CanEvents",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CanTraining",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanEvents",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CanTraining",
                table: "Users");

            migrationBuilder.AddColumn<double>(
                name: "SixMonthHours",
                table: "Warnings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Hours_UserId",
                table: "Hours",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hours_Users_UserId",
                table: "Hours",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
