using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations.SchoolAITech
{
    /// <inheritdoc />
    public partial class AddSchoolExamResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolExamResults",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchoolRegistrationId = table.Column<long>(type: "bigint", nullable: false),
                    TenantKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExamKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    CorrectCount = table.Column<int>(type: "int", nullable: false),
                    WrongCount = table.Column<int>(type: "int", nullable: false),
                    BlankCount = table.Column<int>(type: "int", nullable: false),
                    ScorePercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: true),
                    AcceptanceMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionScoresJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntrySource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OmrConfidence = table.Column<decimal>(type: "decimal(5,3)", precision: 5, scale: 3, nullable: true),
                    OmrReviewRequired = table.Column<bool>(type: "bit", nullable: false),
                    RawOmrJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolExamResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SchoolExamResults_SchoolRegistrations_SchoolRegistrationId",
                        column: x => x.SchoolRegistrationId,
                        principalSchema: "dbo",
                        principalTable: "SchoolRegistrations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolExamResults_SchoolRegistrationId_ExamKey",
                schema: "dbo",
                table: "SchoolExamResults",
                columns: new[] { "SchoolRegistrationId", "ExamKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolExamResults",
                schema: "dbo");
        }
    }
}
