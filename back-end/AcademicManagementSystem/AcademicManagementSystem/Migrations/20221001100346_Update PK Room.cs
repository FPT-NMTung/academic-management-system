using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class UpdatePKRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_room",
                table: "room");

            migrationBuilder.DropColumn(
                name: "room_code",
                table: "room");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "room",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_room",
                table: "room",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_room",
                table: "room");

            migrationBuilder.DropColumn(
                name: "id",
                table: "room");

            migrationBuilder.AddColumn<string>(
                name: "room_code",
                table: "room",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_room",
                table: "room",
                column: "room_code");
        }
    }
}
