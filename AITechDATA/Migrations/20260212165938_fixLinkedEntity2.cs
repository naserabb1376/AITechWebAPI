using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class fixLinkedEntity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "LinkedEntities",
                newName: "SourceTableName");

            migrationBuilder.RenameColumn(
                name: "LinkedEntityId",
                table: "LinkedEntities",
                newName: "SourceRowId");

            migrationBuilder.RenameColumn(
                name: "ForeignKeyId",
                table: "LinkedEntities",
                newName: "DestRowId");

            migrationBuilder.AddColumn<string>(
                name: "DestTableName",
                table: "LinkedEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestTableName",
                table: "LinkedEntities");

            migrationBuilder.RenameColumn(
                name: "SourceTableName",
                table: "LinkedEntities",
                newName: "TableName");

            migrationBuilder.RenameColumn(
                name: "SourceRowId",
                table: "LinkedEntities",
                newName: "LinkedEntityId");

            migrationBuilder.RenameColumn(
                name: "DestRowId",
                table: "LinkedEntities",
                newName: "ForeignKeyId");
        }
    }
}
