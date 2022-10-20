using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class AddRelationshipBetweenSroAndClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sro_id",
                table: "class",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_class_sro_id",
                table: "class",
                column: "sro_id");

            migrationBuilder.AddForeignKey(
                name: "FK_class_sro_sro_id",
                table: "class",
                column: "sro_id",
                principalTable: "sro",
                principalColumn: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_sro_sro_id",
                table: "class");

            migrationBuilder.DropIndex(
                name: "IX_class_sro_id",
                table: "class");

            migrationBuilder.DropColumn(
                name: "sro_id",
                table: "class");
        }
    }
}
