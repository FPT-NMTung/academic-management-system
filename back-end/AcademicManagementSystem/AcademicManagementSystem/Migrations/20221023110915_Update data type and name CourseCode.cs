using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class UpdatedatatypeandnameCourseCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_course_CourseCode",
                table: "student");

            migrationBuilder.DropColumn(
                name: "course_id",
                table: "student");

            migrationBuilder.RenameColumn(
                name: "CourseCode",
                table: "student",
                newName: "course_code");

            migrationBuilder.RenameIndex(
                name: "IX_student_CourseCode",
                table: "student",
                newName: "IX_student_course_code");

            migrationBuilder.AlterColumn<string>(
                name: "course_code",
                table: "student",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_student_course_course_code",
                table: "student",
                column: "course_code",
                principalTable: "course",
                principalColumn: "code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_course_course_code",
                table: "student");

            migrationBuilder.RenameColumn(
                name: "course_code",
                table: "student",
                newName: "CourseCode");

            migrationBuilder.RenameIndex(
                name: "IX_student_course_code",
                table: "student",
                newName: "IX_student_CourseCode");

            migrationBuilder.AlterColumn<string>(
                name: "CourseCode",
                table: "student",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "course_id",
                table: "student",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_student_course_CourseCode",
                table: "student",
                column: "CourseCode",
                principalTable: "course",
                principalColumn: "code");
        }
    }
}
