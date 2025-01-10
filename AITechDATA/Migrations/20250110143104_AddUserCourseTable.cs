using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCourseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_UserID",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_UserID1",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_UserID",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_UserID1",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UserID1",
                table: "Courses");

            migrationBuilder.CreateTable(
                name: "UserCourses",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    PeresentType = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCourses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCourses_CourseId",
                table: "UserCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourses_UserId",
                table: "UserCourses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCourses");

            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "Courses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserID1",
                table: "Courses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_UserID",
                table: "Courses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_UserID1",
                table: "Courses",
                column: "UserID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_UserID",
                table: "Courses",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_UserID1",
                table: "Courses",
                column: "UserID1",
                principalTable: "Users",
                principalColumn: "ID");
        }
    }
}
