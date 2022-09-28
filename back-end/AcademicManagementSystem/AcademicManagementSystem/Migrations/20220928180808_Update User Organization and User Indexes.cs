using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class UpdateUserOrganizationandUserIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_email_mobile_phone_email_company_citizen_identity_card_no",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "email_company",
                table: "user",
                newName: "email_organization");

            migrationBuilder.CreateIndex(
                name: "IX_user_citizen_identity_card_no",
                table: "user",
                column: "citizen_identity_card_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_email_organization",
                table: "user",
                column: "email_organization",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_mobile_phone",
                table: "user",
                column: "mobile_phone",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_citizen_identity_card_no",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_email",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_email_organization",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_mobile_phone",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "email_organization",
                table: "user",
                newName: "email_company");

            migrationBuilder.CreateIndex(
                name: "IX_user_email_mobile_phone_email_company_citizen_identity_card_no",
                table: "user",
                columns: new[] { "email", "mobile_phone", "email_company", "citizen_identity_card_no" },
                unique: true);
        }
    }
}
