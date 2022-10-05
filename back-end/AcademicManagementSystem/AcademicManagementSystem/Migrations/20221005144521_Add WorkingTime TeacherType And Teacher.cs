using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddWorkingTimeTeacherTypeAndTeacher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "teacher_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teacher_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "working_time",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_working_time", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "teacher",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    teacher_type_id = table.Column<int>(type: "int", nullable: false),
                    working_time_id = table.Column<int>(type: "int", nullable: false),
                    nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    company_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_working_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tax_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teacher", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_teacher_teacher_type_teacher_type_id",
                        column: x => x.teacher_type_id,
                        principalTable: "teacher_type",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_teacher_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_teacher_working_time_working_time_id",
                        column: x => x.working_time_id,
                        principalTable: "working_time",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_teacher_teacher_type_id",
                table: "teacher",
                column: "teacher_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_working_time_id",
                table: "teacher",
                column: "working_time_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "teacher");

            migrationBuilder.DropTable(
                name: "teacher_type");

            migrationBuilder.DropTable(
                name: "working_time");
        }
    }
}
