using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddSessionAndSessionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "session_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    class_schedule_id = table.Column<int>(type: "int", nullable: false),
                    session_type_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    learning_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session", x => x.id);
                    table.ForeignKey(
                        name: "FK_session_class_schedule_class_schedule_id",
                        column: x => x.class_schedule_id,
                        principalTable: "class_schedule",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_session_session_type_session_type_id",
                        column: x => x.session_type_id,
                        principalTable: "session_type",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_session_class_schedule_id",
                table: "session",
                column: "class_schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_session_type_id",
                table: "session",
                column: "session_type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "session_type");
        }
    }
}
