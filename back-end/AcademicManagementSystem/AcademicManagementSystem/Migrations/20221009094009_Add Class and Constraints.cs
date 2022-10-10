using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddClassandConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "class_days",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_class_days", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "class_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_class_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "class",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    center_id = table.Column<int>(type: "int", nullable: false),
                    course_code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    class_days_id = table.Column<int>(type: "int", nullable: false),
                    class_status_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    completion_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    graduation_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    class_hour_start = table.Column<TimeSpan>(type: "time", nullable: false),
                    class_hour_end = table.Column<TimeSpan>(type: "time", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_class", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_center_center_id",
                        column: x => x.center_id,
                        principalTable: "center",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_class_days_class_days_id",
                        column: x => x.class_days_id,
                        principalTable: "class_days",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_class_status_class_status_id",
                        column: x => x.class_status_id,
                        principalTable: "class_status",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_class_course_course_code",
                        column: x => x.course_code,
                        principalTable: "course",
                        principalColumn: "code");
                });

            migrationBuilder.CreateIndex(
                name: "IX_class_center_id",
                table: "class",
                column: "center_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_class_days_id",
                table: "class",
                column: "class_days_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_class_status_id",
                table: "class",
                column: "class_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_course_code",
                table: "class",
                column: "course_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "class");

            migrationBuilder.DropTable(
                name: "class_days");

            migrationBuilder.DropTable(
                name: "class_status");
        }
    }
}
