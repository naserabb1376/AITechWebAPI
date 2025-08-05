using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class addJobRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "UserGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "UserCourses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "TicketMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "TeacherResumes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "StudentDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "SessionAssignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "PreRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissionType",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "PermissionRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "LoginMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "FileUploads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "AdminReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtherLangs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Articles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedPosition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtherLangs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRequests", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LinkedEntities",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LinkType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForeignKeyId = table.Column<long>(type: "bigint", nullable: false),
                    LinkedEntityId = table.Column<long>(type: "bigint", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OtherLangs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkedEntities", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "JobRequests");

            migrationBuilder.DropTable(
                name: "LinkedEntities");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "UserCourses");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "TicketMessages");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "TeacherResumes");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "StudentDetails");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "SessionAssignments");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "PreRegistrations");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "PermissionType",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "PermissionRoles");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "News");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "LoginMethods");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "AdminReports");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "Addresses");
        }
    }
}
