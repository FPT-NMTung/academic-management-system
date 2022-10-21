using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddStudentAndStudentClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    enroll_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    status_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    home_phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    contact_phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    parental_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    parental_relationship = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    contact_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    parental_phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    application_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    application_document = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    high_school = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    university = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    facebook_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    portfolio_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    working_company = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    company_salary = table.Column<int>(type: "int", nullable: true),
                    company_position = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    company_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    fee_plan = table.Column<int>(type: "int", nullable: false),
                    promotion = table.Column<int>(type: "int", nullable: false),
                    is_draft = table.Column<bool>(type: "bit", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_student_course_CourseCode",
                        column: x => x.CourseCode,
                        principalTable: "course",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "FK_student_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "student_class",
                columns: table => new
                {
                    student_id = table.Column<int>(type: "int", nullable: false),
                    class_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_class", x => new { x.student_id, x.class_id });
                    table.ForeignKey(
                        name: "FK_student_class_class_class_id",
                        column: x => x.class_id,
                        principalTable: "class",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_student_class_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_CourseCode",
                table: "student",
                column: "CourseCode");

            migrationBuilder.CreateIndex(
                name: "IX_student_enroll_number",
                table: "student",
                column: "enroll_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_class_class_id",
                table: "student_class",
                column: "class_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_class");

            migrationBuilder.DropTable(
                name: "student");
        }
    }
}
