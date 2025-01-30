using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class fixDb5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRoles_Permissions_RoleId",
                table: "PermissionRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRoles_PerrmissionId",
                table: "PermissionRoles",
                column: "PerrmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRoles_Permissions_PerrmissionId",
                table: "PermissionRoles",
                column: "PerrmissionId",
                principalTable: "Permissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRoles_Permissions_PerrmissionId",
                table: "PermissionRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRoles_PerrmissionId",
                table: "PermissionRoles");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRoles_Permissions_RoleId",
                table: "PermissionRoles",
                column: "RoleId",
                principalTable: "Permissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
