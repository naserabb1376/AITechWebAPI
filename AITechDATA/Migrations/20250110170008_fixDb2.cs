using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class fixDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreRegistrations_Courses_CourseId",
                table: "PreRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_PreRegistrations_Groups_GroupID",
                table: "PreRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_GroupID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GroupID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PreRegistrations_CourseId",
                table: "PreRegistrations");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "PreRegistrations");

            migrationBuilder.RenameColumn(
                name: "GroupID",
                table: "PreRegistrations",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_PreRegistrations_GroupID",
                table: "PreRegistrations",
                newName: "IX_PreRegistrations_GroupId");

            migrationBuilder.AlterColumn<long>(
                name: "GroupId",
                table: "PreRegistrations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "News",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Events",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_News_UserId",
                table: "News",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_News_Users_UserId",
                table: "News",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreRegistrations_Groups_GroupId",
                table: "PreRegistrations",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Users_UserId",
                table: "News");

            migrationBuilder.DropForeignKey(
                name: "FK_PreRegistrations_Groups_GroupId",
                table: "PreRegistrations");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropIndex(
                name: "IX_News_UserId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_Events_UserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "PreRegistrations",
                newName: "GroupID");

            migrationBuilder.RenameIndex(
                name: "IX_PreRegistrations_GroupId",
                table: "PreRegistrations",
                newName: "IX_PreRegistrations_GroupID");

            migrationBuilder.AddColumn<long>(
                name: "GroupID",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "GroupID",
                table: "PreRegistrations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                table: "PreRegistrations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupID",
                table: "Users",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrations_CourseId",
                table: "PreRegistrations",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreRegistrations_Courses_CourseId",
                table: "PreRegistrations",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreRegistrations_Groups_GroupID",
                table: "PreRegistrations",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_GroupID",
                table: "Users",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "ID");
        }
    }
}
