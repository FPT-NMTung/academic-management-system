using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddGpaRecordAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerGpaRecord");

            migrationBuilder.CreateTable(
                name: "gpa_record_answer",
                columns: table => new
                {
                    gpa_record_id = table.Column<int>(type: "int", nullable: false),
                    answer_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gpa_record_answer", x => new { x.answer_id, x.gpa_record_id });
                    table.ForeignKey(
                        name: "FK_gpa_record_answer_answer_answer_id",
                        column: x => x.answer_id,
                        principalTable: "answer",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_gpa_record_answer_gpa_record_gpa_record_id",
                        column: x => x.gpa_record_id,
                        principalTable: "gpa_record",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_gpa_record_answer_gpa_record_id",
                table: "gpa_record_answer",
                column: "gpa_record_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gpa_record_answer");

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
                name: "IX_AnswerGpaRecord_GpaRecordsId",
                table: "AnswerGpaRecord",
                column: "GpaRecordsId");
        }
    }
}
