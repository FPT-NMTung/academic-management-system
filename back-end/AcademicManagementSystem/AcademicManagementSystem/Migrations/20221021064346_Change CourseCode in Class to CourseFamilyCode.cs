using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class ChangeCourseCodeinClasstoCourseFamilyCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_course_course_code",
                table: "class");

            migrationBuilder.RenameColumn(
                name: "course_code",
                table: "class",
                newName: "course_family_code");

            migrationBuilder.RenameIndex(
                name: "IX_class_course_code",
                table: "class",
                newName: "IX_class_course_family_code");

            migrationBuilder.AddForeignKey(
                name: "FK_class_course_family_course_family_code",
                table: "class",
                column: "course_family_code",
                principalTable: "course_family",
                principalColumn: "code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_course_family_course_family_code",
                table: "class");

            migrationBuilder.RenameColumn(
                name: "course_family_code",
                table: "class",
                newName: "course_code");

            migrationBuilder.RenameIndex(
                name: "IX_class_course_family_code",
                table: "class",
                newName: "IX_class_course_code");

            migrationBuilder.AddForeignKey(
                name: "FK_class_course_course_code",
                table: "class",
                column: "course_code",
                principalTable: "course",
                principalColumn: "code");
        }
    }
}
