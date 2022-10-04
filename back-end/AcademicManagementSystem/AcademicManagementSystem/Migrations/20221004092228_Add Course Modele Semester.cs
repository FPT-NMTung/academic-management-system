using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddCourseModeleSemester : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_family",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    published_year = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_family", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "module",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    center_id = table.Column<int>(type: "int", nullable: false),
                    semester_name_portal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    module_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    module_exam_name_portal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    module_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    max_theory_grade = table.Column<int>(type: "int", nullable: true),
                    max_practical_grade = table.Column<int>(type: "int", nullable: true),
                    hours = table.Column<int>(type: "int", nullable: false),
                    days = table.Column<int>(type: "int", nullable: false),
                    exam_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_module", x => x.id);
                    table.ForeignKey(
                        name: "FK_module_center_center_id",
                        column: x => x.center_id,
                        principalTable: "center",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "semester",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_semester", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "course",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    course_family_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    semester_count = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course", x => x.code);
                    table.ForeignKey(
                        name: "FK_course_course_family_course_family_code",
                        column: x => x.course_family_code,
                        principalTable: "course_family",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "course_module_semester",
                columns: table => new
                {
                    course_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    module_id = table.Column<int>(type: "int", nullable: false),
                    semester_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_module_semester", x => new { x.course_code, x.module_id, x.semester_id });
                    table.ForeignKey(
                        name: "FK_course_module_semester_course_course_code",
                        column: x => x.course_code,
                        principalTable: "course",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "FK_course_module_semester_module_module_id",
                        column: x => x.module_id,
                        principalTable: "module",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_course_module_semester_semester_semester_id",
                        column: x => x.semester_id,
                        principalTable: "semester",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_course_family_code",
                table: "course",
                column: "course_family_code");

            migrationBuilder.CreateIndex(
                name: "IX_course_module_semester_module_id",
                table: "course_module_semester",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_module_semester_semester_id",
                table: "course_module_semester",
                column: "semester_id");

            migrationBuilder.CreateIndex(
                name: "IX_module_center_id",
                table: "module",
                column: "center_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_module_semester");

            migrationBuilder.DropTable(
                name: "course");

            migrationBuilder.DropTable(
                name: "module");

            migrationBuilder.DropTable(
                name: "semester");

            migrationBuilder.DropTable(
                name: "course_family");
        }
    }
}
