using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddCenter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "center",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    province_id = table.Column<int>(type: "int", nullable: false),
                    district_id = table.Column<int>(type: "int", nullable: false),
                    ward_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_center", x => x.id);
                    table.ForeignKey(
                        name: "FK_center_district_district_id",
                        column: x => x.district_id,
                        principalTable: "district",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_center_province_province_id",
                        column: x => x.province_id,
                        principalTable: "province",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_center_ward_ward_id",
                        column: x => x.ward_id,
                        principalTable: "ward",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_center_district_id",
                table: "center",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_center_province_id",
                table: "center",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "IX_center_ward_id",
                table: "center",
                column: "ward_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "center");
        }
    }
}
