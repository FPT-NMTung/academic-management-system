using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddAttendanceAndAttendanceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attendance_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "attendance",
                columns: table => new
                {
                    session_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AttendanceStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance", x => new { x.session_id, x.student_id });
                    table.ForeignKey(
                        name: "FK_attendance_attendance_status_AttendanceStatusId",
                        column: x => x.AttendanceStatusId,
                        principalTable: "attendance_status",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_attendance_session_session_id",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_attendance_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_attendance_AttendanceStatusId",
                table: "attendance",
                column: "AttendanceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_student_id",
                table: "attendance",
                column: "student_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendance");

            migrationBuilder.DropTable(
                name: "attendance_status");
        }
    }
}
