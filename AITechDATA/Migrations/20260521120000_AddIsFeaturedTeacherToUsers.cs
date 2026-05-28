using AITechDATA.DataLayer;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    [DbContext(typeof(AITechContext))]
    [Migration("20260521120000_AddIsFeaturedTeacherToUsers")]
    public partial class AddIsFeaturedTeacherToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeaturedTeacher",
                table: "Users",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeaturedTeacher",
                table: "Users");
        }
    }
}
