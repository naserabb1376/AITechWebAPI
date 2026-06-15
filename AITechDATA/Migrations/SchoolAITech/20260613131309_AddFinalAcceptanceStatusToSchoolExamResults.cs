using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations.SchoolAITech
{
    /// <inheritdoc />
    public partial class AddFinalAcceptanceStatusToSchoolExamResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinalAcceptanceStatus",
                schema: "dbo",
                table: "SchoolExamResults",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalAcceptanceStatus",
                schema: "dbo",
                table: "SchoolExamResults");
        }
    }
}
