using System;
using AITechDATA.DataLayer;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    [DbContext(typeof(AITechContext))]
    [Migration("20260523120000_AddSchoolRegistrations")]
    public partial class AddSchoolRegistrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolRegistrations",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StudentDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    FatherParentId = table.Column<long>(type: "bigint", nullable: false),
                    MotherParentId = table.Column<long>(type: "bigint", nullable: false),
                    TenantKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentSchoolName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetGrade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolRegistrations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SchoolRegistrations_Parents_FatherParentId",
                        column: x => x.FatherParentId,
                        principalTable: "Parents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchoolRegistrations_Parents_MotherParentId",
                        column: x => x.MotherParentId,
                        principalTable: "Parents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchoolRegistrations_StudentDetails_StudentDetailsId",
                        column: x => x.StudentDetailsId,
                        principalTable: "StudentDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolRegistrations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolRegistrations_FatherParentId",
                table: "SchoolRegistrations",
                column: "FatherParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolRegistrations_MotherParentId",
                table: "SchoolRegistrations",
                column: "MotherParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolRegistrations_StudentDetailsId",
                table: "SchoolRegistrations",
                column: "StudentDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolRegistrations_UserId",
                table: "SchoolRegistrations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolRegistrations");
        }
    }
}
