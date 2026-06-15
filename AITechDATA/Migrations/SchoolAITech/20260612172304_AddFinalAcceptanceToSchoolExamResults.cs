using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations.SchoolAITech
{
    /// <inheritdoc />
    public partial class AddFinalAcceptanceToSchoolExamResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinalAccepted",
                schema: "dbo",
                table: "SchoolExamResults",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinalAccepted",
                schema: "dbo",
                table: "SchoolExamResults");
        }
    }
}
