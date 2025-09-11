using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class GlobalizePreRegistration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreRegistrations_Groups_GroupId",
                table: "PreRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_PreRegistrations_GroupId",
                table: "PreRegistrations");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "PreRegistrations",
                newName: "ForeignKeyId");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "PreRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "PreRegistrations");

            migrationBuilder.RenameColumn(
                name: "ForeignKeyId",
                table: "PreRegistrations",
                newName: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PreRegistrations_GroupId",
                table: "PreRegistrations",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreRegistrations_Groups_GroupId",
                table: "PreRegistrations",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
