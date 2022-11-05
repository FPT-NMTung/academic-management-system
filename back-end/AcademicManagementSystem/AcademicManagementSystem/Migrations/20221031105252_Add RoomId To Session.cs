using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddRoomIdToSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "room",
                table: "session");

            migrationBuilder.AddColumn<int>(
                name: "room_id",
                table: "session",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<int>(
                name: "theory_room_id",
                table: "class_schedule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "lab_room_id",
                table: "class_schedule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "exam_room_id",
                table: "class_schedule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_session_room_id",
                table: "session",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "FK_session_room_room_id",
                table: "session",
                column: "room_id",
                principalTable: "room",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_session_room_room_id",
                table: "session");

            migrationBuilder.DropIndex(
                name: "IX_session_room_id",
                table: "session");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "session");

            migrationBuilder.AddColumn<string>(
                name: "room",
                table: "session",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<int>(
                name: "theory_room_id",
                table: "class_schedule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "lab_room_id",
                table: "class_schedule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "exam_room_id",
                table: "class_schedule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
