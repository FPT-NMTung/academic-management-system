using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddWorkingTimeToClassSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "working_hour_id",
                table: "day_off",
                newName: "working_time_id");

            migrationBuilder.AddColumn<int>(
                name: "working_time_id",
                table: "class_schedule",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 5);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "working_time_id",
                table: "class_schedule");

            migrationBuilder.RenameColumn(
                name: "working_time_id",
                table: "day_off",
                newName: "working_hour_id");
        }
    }
}
