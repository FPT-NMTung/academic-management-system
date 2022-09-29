using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademicManagementSystem.Migrations
{
    public partial class Updateandfixdatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    province_id = table.Column<int>(type: "int", nullable: false),
                    district_id = table.Column<int>(type: "int", nullable: false),
                    ward_id = table.Column<int>(type: "int", nullable: false),
                    center_id = table.Column<int>(type: "int", nullable: false),
                    gender_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    avatar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    mobile_phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email_organization = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    birthday = table.Column<DateTime>(type: "date", nullable: false),
                    citizen_identity_card_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    citizen_identity_card_published_date = table.Column<DateTime>(type: "date", nullable: false),
                    citizen_identity_card_published_place = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_center_center_id",
                        column: x => x.center_id,
                        principalTable: "center",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_district_district_id",
                        column: x => x.district_id,
                        principalTable: "district",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_gender_gender_id",
                        column: x => x.gender_id,
                        principalTable: "gender",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_province_province_id",
                        column: x => x.province_id,
                        principalTable: "province",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_ward_ward_id",
                        column: x => x.ward_id,
                        principalTable: "ward",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    room_code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    center_id = table.Column<int>(type: "int", nullable: false),
                    room_type_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room", x => x.room_code);
                    table.ForeignKey(
                        name: "FK_room_center_center_id",
                        column: x => x.center_id,
                        principalTable: "center",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_room_room_type_room_type_id",
                        column: x => x.room_type_id,
                        principalTable: "room_type",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "active_refresh_token",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    exp_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_active_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_active_refresh_token_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_admin_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sro",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sro", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_sro_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_active_refresh_token_user_id",
                table: "active_refresh_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_room_center_id",
                table: "room",
                column: "center_id");

            migrationBuilder.CreateIndex(
                name: "IX_room_room_type_id",
                table: "room",
                column: "room_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_center_id",
                table: "user",
                column: "center_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_citizen_identity_card_no",
                table: "user",
                column: "citizen_identity_card_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_district_id",
                table: "user",
                column: "district_id");

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
                name: "IX_user_gender_id",
                table: "user",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_mobile_phone",
                table: "user",
                column: "mobile_phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_province_id",
                table: "user",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_ward_id",
                table: "user",
                column: "ward_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "active_refresh_token");

            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "sro");

            migrationBuilder.DropTable(
                name: "room_type");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
