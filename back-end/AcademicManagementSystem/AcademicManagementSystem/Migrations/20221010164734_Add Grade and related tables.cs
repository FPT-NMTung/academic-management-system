using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddGradeandrelatedtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "grade_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "grade_category_module",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    module_id = table.Column<int>(type: "int", nullable: false),
                    grade_category_id = table.Column<int>(type: "int", nullable: false),
                    total_weight = table.Column<int>(type: "int", nullable: false),
                    quantity_grade_item = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_category_module", x => x.id);
                    table.ForeignKey(
                        name: "FK_grade_category_module_grade_category_grade_category_id",
                        column: x => x.grade_category_id,
                        principalTable: "grade_category",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_grade_category_module_module_module_id",
                        column: x => x.module_id,
                        principalTable: "module",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "grade_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    grade_category_module_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grade_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_grade_item_grade_category_module_grade_category_module_id",
                        column: x => x.grade_category_module_id,
                        principalTable: "grade_category_module",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_grade_category_module_grade_category_id",
                table: "grade_category_module",
                column: "grade_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_grade_category_module_module_id",
                table: "grade_category_module",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_grade_item_grade_category_module_id",
                table: "grade_item",
                column: "grade_category_module_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grade_item");

            migrationBuilder.DropTable(
                name: "grade_category_module");

            migrationBuilder.DropTable(
                name: "grade_category");
        }
    }
}
