using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddClassSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "class_schedule",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    class_id = table.Column<int>(type: "int", nullable: false),
                    module_id = table.Column<int>(type: "int", nullable: false),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    class_days_id = table.Column<int>(type: "int", nullable: false),
                    class_status_id = table.Column<int>(type: "int", nullable: false),
                    theory_room_id = table.Column<int>(type: "int", nullable: false),
                    lab_room_id = table.Column<int>(type: "int", nullable: false),
                    exam_room_id = table.Column<int>(type: "int", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    theory_exam_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    practical_exam_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    class_hour_start = table.Column<TimeSpan>(type: "time", nullable: false),
                    class_hour_end = table.Column<TimeSpan>(type: "time", nullable: false),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_class_schedule", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_schedule_class_class_id",
                        column: x => x.class_id,
                        principalTable: "class",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_schedule_class_days_class_days_id",
                        column: x => x.class_days_id,
                        principalTable: "class_days",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_schedule_class_status_class_status_id",
                        column: x => x.class_status_id,
                        principalTable: "class_status",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_schedule_module_module_id",
                        column: x => x.module_id,
                        principalTable: "module",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_schedule_room_exam_room_id",
                        column: x => x.exam_room_id,
                        principalTable: "room",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_schedule_room_lab_room_id",
                        column: x => x.lab_room_id,
                        principalTable: "room",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_schedule_room_theory_room_id",
                        column: x => x.theory_room_id,
                        principalTable: "room",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_schedule_teacher_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "teacher",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_class_schedule_class_days_id",
                table: "class_schedule",
                column: "class_days_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_schedule_class_id",
                table: "class_schedule",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_schedule_class_status_id",
                table: "class_schedule",
                column: "class_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_schedule_exam_room_id",
                table: "class_schedule",
                column: "exam_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_schedule_lab_room_id",
                table: "class_schedule",
                column: "lab_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_schedule_module_id",
                table: "class_schedule",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_schedule_teacher_id",
                table: "class_schedule",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_schedule_theory_room_id",
                table: "class_schedule",
                column: "theory_room_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "class_schedule");
        }
    }
}
