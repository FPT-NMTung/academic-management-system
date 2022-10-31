using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddGPAandRelatedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attendance_attendance_status_AttendanceStatusId",
                table: "attendance");

            migrationBuilder.RenameColumn(
                name: "AttendanceStatusId",
                table: "attendance",
                newName: "attendance_status_id");

            migrationBuilder.RenameIndex(
                name: "IX_attendance_AttendanceStatusId",
                table: "attendance",
                newName: "IX_attendance_attendance_status_id");

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "student_grade",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "status_date",
                table: "student",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "application_date",
                table: "student",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "class",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "graduation_date",
                table: "class",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "completion_date",
                table: "class",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "day_off",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacher_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_day_off", x => x.id);
                    table.ForeignKey(
                        name: "FK_day_off_teacher_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "teacher",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "form",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gpa_record",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    class_id = table.Column<int>(type: "int", nullable: false),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    module_id = table.Column<int>(type: "int", nullable: false),
                    session_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gpa_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_gpa_record_class_class_id",
                        column: x => x.class_id,
                        principalTable: "class",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_gpa_record_form_form_id",
                        column: x => x.form_id,
                        principalTable: "form",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_gpa_record_module_module_id",
                        column: x => x.module_id,
                        principalTable: "module",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_gpa_record_session_session_id",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_gpa_record_student_student_id",
                        column: x => x.student_id,
                        principalTable: "student",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_gpa_record_teacher_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "teacher",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "answer",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    answer_no = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answer", x => x.id);
                    table.ForeignKey(
                        name: "FK_answer_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "FormQuestion",
                columns: table => new
                {
                    FormsId = table.Column<int>(type: "int", nullable: false),
                    QuestionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormQuestion", x => new { x.FormsId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_FormQuestion_form_FormsId",
                        column: x => x.FormsId,
                        principalTable: "form",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_FormQuestion_question_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "question",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "AnswerGpaRecord",
                columns: table => new
                {
                    AnswersId = table.Column<int>(type: "int", nullable: false),
                    GpaRecordsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerGpaRecord", x => new { x.AnswersId, x.GpaRecordsId });
                    table.ForeignKey(
                        name: "FK_AnswerGpaRecord_answer_AnswersId",
                        column: x => x.AnswersId,
                        principalTable: "answer",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AnswerGpaRecord_gpa_record_GpaRecordsId",
                        column: x => x.GpaRecordsId,
                        principalTable: "gpa_record",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_answer_question_id",
                table: "answer",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerGpaRecord_GpaRecordsId",
                table: "AnswerGpaRecord",
                column: "GpaRecordsId");

            migrationBuilder.CreateIndex(
                name: "IX_day_off_teacher_id",
                table: "day_off",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormQuestion_QuestionsId",
                table: "FormQuestion",
                column: "QuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_gpa_record_class_id",
                table: "gpa_record",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_gpa_record_form_id",
                table: "gpa_record",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_gpa_record_module_id",
                table: "gpa_record",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_gpa_record_session_id",
                table: "gpa_record",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_gpa_record_student_id",
                table: "gpa_record",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_gpa_record_teacher_id",
                table: "gpa_record",
                column: "teacher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_attendance_status_attendance_status_id",
                table: "attendance",
                column: "attendance_status_id",
                principalTable: "attendance_status",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attendance_attendance_status_attendance_status_id",
                table: "attendance");

            migrationBuilder.DropTable(
                name: "AnswerGpaRecord");

            migrationBuilder.DropTable(
                name: "day_off");

            migrationBuilder.DropTable(
                name: "FormQuestion");

            migrationBuilder.DropTable(
                name: "answer");

            migrationBuilder.DropTable(
                name: "gpa_record");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "form");

            migrationBuilder.RenameColumn(
                name: "attendance_status_id",
                table: "attendance",
                newName: "AttendanceStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_attendance_attendance_status_id",
                table: "attendance",
                newName: "IX_attendance_AttendanceStatusId");

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "student_grade",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "status_date",
                table: "student",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "application_date",
                table: "student",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "class",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "graduation_date",
                table: "class",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "completion_date",
                table: "class",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_attendance_status_AttendanceStatusId",
                table: "attendance",
                column: "AttendanceStatusId",
                principalTable: "attendance_status",
                principalColumn: "id");
        }
    }
}
