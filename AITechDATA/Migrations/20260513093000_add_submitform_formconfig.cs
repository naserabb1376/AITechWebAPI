using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class add_submitform_formconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[SubmitForms]', N'FormConfig') IS NULL
BEGIN
    ALTER TABLE [SubmitForms] ADD [FormConfig] nvarchar(max) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[SubmitForms]', N'FormConfig') IS NOT NULL
BEGIN
    ALTER TABLE [SubmitForms] DROP COLUMN [FormConfig];
END
");
        }
    }
}
