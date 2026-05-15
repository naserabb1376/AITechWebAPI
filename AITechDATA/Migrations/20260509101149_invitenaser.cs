using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class invitenaser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdentificationCode",
                table: "Users",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InvitationRewardApplied",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvitationRewardAppliedDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InviterUserId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE [Users]
SET [IdentificationCode] = CONCAT(N'AITech', [ID] + 1000)
WHERE [ID] IS NOT NULL;
");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentificationCode",
                table: "Users",
                column: "IdentificationCode",
                unique: true,
                filter: "[IdentificationCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_InviterUserId",
                table: "Users",
                column: "InviterUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IdentificationCode",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_InviterUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InvitationRewardApplied",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InvitationRewardAppliedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InviterUserId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "IdentificationCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
