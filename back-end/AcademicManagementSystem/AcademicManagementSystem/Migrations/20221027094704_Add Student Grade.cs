using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddStudentGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_grade",
                columns: table => new
                {
                    class_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    grade_item_id = table.Column<int>(type: "int", nullable: false),
                    grade = table.Column<double>(type: "float", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_grade", x => new { x.class_id, x.student_id, x.grade_item_id });
                    table.ForeignKey(
                        name: "FK_student_grade_class_class_id",
                        column: x => x.class_id,
                        principalTable: "class",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_student_grade_grade_item_grade_item_id",
                        column: x => x.grade_item_id,
                        principalTable: "grade_item",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_student_grade_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_grade_grade_item_id",
                table: "student_grade",
                column: "grade_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_grade_student_id",
                table: "student_grade",
                column: "student_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_grade");
        }
    }
}
