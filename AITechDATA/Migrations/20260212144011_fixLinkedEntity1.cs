using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class fixLinkedEntity1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "LinkedEntities");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "LinkedEntities");

            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "LinkedEntities",
                newName: "TableName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "LinkedEntities",
                newName: "EntityName");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "LinkedEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "LinkedEntities",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
