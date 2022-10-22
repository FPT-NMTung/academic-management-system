using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class FixTeacherSkill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "teacher_skill");

            migrationBuilder.CreateTable(
                name: "SkillTeacher",
                columns: table => new
                {
                    SkillsId = table.Column<int>(type: "int", nullable: false),
                    TeachersUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTeacher", x => new { x.SkillsId, x.TeachersUserId });
                    table.ForeignKey(
                        name: "FK_SkillTeacher_skill_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "skill",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_SkillTeacher_teacher_TeachersUserId",
                        column: x => x.TeachersUserId,
                        principalTable: "teacher",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillTeacher_TeachersUserId",
                table: "SkillTeacher",
                column: "TeachersUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillTeacher");

            migrationBuilder.CreateTable(
                name: "teacher_skill",
                columns: table => new
                {
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    skill_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teacher_skill", x => new { x.teacher_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_teacher_skill_skill_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skill",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_teacher_skill_teacher_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "teacher",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_teacher_skill_skill_id",
                table: "teacher_skill",
                column: "skill_id");
        }
    }
}
