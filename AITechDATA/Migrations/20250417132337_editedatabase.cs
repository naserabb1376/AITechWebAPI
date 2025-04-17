using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class editedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileUploads_Assignments_AssignmentId",
                table: "FileUploads");

            migrationBuilder.DropIndex(
                name: "IX_FileUploads_AssignmentId",
                table: "FileUploads");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "FileUploads",
                newName: "ForeignKeyId");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "FileUploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "FileUploads");

            migrationBuilder.RenameColumn(
                name: "ForeignKeyId",
                table: "FileUploads",
                newName: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_AssignmentId",
                table: "FileUploads",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileUploads_Assignments_AssignmentId",
                table: "FileUploads",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
