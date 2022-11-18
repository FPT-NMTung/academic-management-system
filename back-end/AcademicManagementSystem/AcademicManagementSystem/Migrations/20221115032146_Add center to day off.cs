using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class Addcentertodayoff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "center_id",
                table: "day_off",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_day_off_center_id",
                table: "day_off",
                column: "center_id");

            migrationBuilder.AddForeignKey(
                name: "FK_day_off_center_center_id",
                table: "day_off",
                column: "center_id",
                principalTable: "center",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_day_off_center_center_id",
                table: "day_off");

            migrationBuilder.DropIndex(
                name: "IX_day_off_center_id",
                table: "day_off");

            migrationBuilder.DropColumn(
                name: "center_id",
                table: "day_off");
        }
    }
}
