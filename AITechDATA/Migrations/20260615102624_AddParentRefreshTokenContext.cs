using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class AddParentRefreshTokenContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EffectiveRoleId",
                table: "Tokens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginType",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "Tokens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SelectedStudentDetailsId",
                table: "Tokens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StudentUserId",
                table: "Tokens",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveRoleId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "LoginType",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "SelectedStudentDetailsId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "StudentUserId",
                table: "Tokens");
        }
    }
}
