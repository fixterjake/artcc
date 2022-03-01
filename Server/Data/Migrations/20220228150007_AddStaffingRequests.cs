using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ZDC.Server.Migrations
{
    public partial class AddStaffingRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ots_Users_InstructorId",
                table: "Ots");

            migrationBuilder.DropForeignKey(
                name: "FK_Ots_Users_RecommenderId",
                table: "Ots");

            migrationBuilder.DropForeignKey(
                name: "FK_Ots_Users_UserId",
                table: "Ots");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Ots",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RecommenderId",
                table: "Ots",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InstructorId",
                table: "Ots",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "StaffingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Affiliation = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffingRequests", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Ots_Users_InstructorId",
                table: "Ots",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ots_Users_RecommenderId",
                table: "Ots",
                column: "RecommenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ots_Users_UserId",
                table: "Ots",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ots_Users_InstructorId",
                table: "Ots");

            migrationBuilder.DropForeignKey(
                name: "FK_Ots_Users_RecommenderId",
                table: "Ots");

            migrationBuilder.DropForeignKey(
                name: "FK_Ots_Users_UserId",
                table: "Ots");

            migrationBuilder.DropTable(
                name: "StaffingRequests");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Ots",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "RecommenderId",
                table: "Ots",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "InstructorId",
                table: "Ots",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Ots_Users_InstructorId",
                table: "Ots",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ots_Users_RecommenderId",
                table: "Ots",
                column: "RecommenderId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ots_Users_UserId",
                table: "Ots",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
