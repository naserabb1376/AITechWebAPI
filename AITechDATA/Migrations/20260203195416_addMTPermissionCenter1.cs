using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class addMTPermissionCenter1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRoles_Permissions_PerrmissionId",
                table: "PermissionRoles");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRoles_RoleId",
                table: "PermissionRoles");

            migrationBuilder.DropColumn(
                name: "Description_EN",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Name_EN",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "OtherLangs",
                table: "PermissionRoles");

            migrationBuilder.RenameColumn(
                name: "PerrmissionId",
                table: "PermissionRoles",
                newName: "PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRoles_PerrmissionId",
                table: "PermissionRoles",
                newName: "IX_PermissionRoles_PermissionId");

            migrationBuilder.AddColumn<long>(
                name: "PermissionsVersion",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Routename",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Permissions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "OwnerOnly",
                table: "PermissionRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    OwnerOnly = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Key",
                table: "Permissions",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRoles_RoleId_PermissionId",
                table: "PermissionRoles",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId_PermissionId",
                table: "UserPermissions",
                columns: new[] { "UserId", "PermissionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRoles_Permissions_PermissionId",
                table: "PermissionRoles",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRoles_Permissions_PermissionId",
                table: "PermissionRoles");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_Key",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRoles_RoleId_PermissionId",
                table: "PermissionRoles");

            migrationBuilder.DropColumn(
                name: "PermissionsVersion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "OwnerOnly",
                table: "PermissionRoles");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "PermissionRoles",
                newName: "PerrmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRoles_PermissionId",
                table: "PermissionRoles",
                newName: "IX_PermissionRoles_PerrmissionId");

            migrationBuilder.AlterColumn<string>(
                name: "Routename",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description_EN",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name_EN",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OtherLangs",
                table: "PermissionRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRoles_RoleId",
                table: "PermissionRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRoles_Permissions_PerrmissionId",
                table: "PermissionRoles",
                column: "PerrmissionId",
                principalTable: "Permissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
