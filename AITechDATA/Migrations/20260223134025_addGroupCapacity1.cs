using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class addGroupCapacity1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupCapacity",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegisterCount",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupCapacity",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "RegisterCount",
                table: "Groups");
        }
    }
}
