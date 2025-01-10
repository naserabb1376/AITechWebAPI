using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class MakeAddressUserRelashionship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentDetails_Addresses_AddressId",
                table: "StudentDetails");

            migrationBuilder.DropIndex(
                name: "IX_StudentDetails_AddressId",
                table: "StudentDetails");

            migrationBuilder.DropColumn(
                name: "ParentInfo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "StudentDetails");

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressId",
                table: "Users",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AddressId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "ParentInfo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                table: "StudentDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_StudentDetails_AddressId",
                table: "StudentDetails",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentDetails_Addresses_AddressId",
                table: "StudentDetails",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
