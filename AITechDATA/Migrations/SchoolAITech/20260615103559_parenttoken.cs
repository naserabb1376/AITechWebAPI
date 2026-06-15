using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations.SchoolAITech
{
    /// <inheritdoc />
    public partial class parenttoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EffectiveRoleId",
                schema: "dbo",
                table: "Tokens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginType",
                schema: "dbo",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                schema: "dbo",
                table: "Tokens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SelectedStudentDetailsId",
                schema: "dbo",
                table: "Tokens",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StudentUserId",
                schema: "dbo",
                table: "Tokens",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SecurePlayerDevices",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceFingerprint = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    FirstActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OtherLangs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurePlayerDevices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SecurePlayerDevices_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecurePlayerViewLogs",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceFingerprint = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OtherLangs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurePlayerViewLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SecurePlayerViewLogs_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "dbo",
                        principalTable: "Sessions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SecurePlayerViewLogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecurePlayerDevices_UserId_DeviceFingerprint",
                schema: "dbo",
                table: "SecurePlayerDevices",
                columns: new[] { "UserId", "DeviceFingerprint" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecurePlayerViewLogs_SessionId",
                schema: "dbo",
                table: "SecurePlayerViewLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurePlayerViewLogs_UserId_SessionId_LoggedAt",
                schema: "dbo",
                table: "SecurePlayerViewLogs",
                columns: new[] { "UserId", "SessionId", "LoggedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecurePlayerDevices",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SecurePlayerViewLogs",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "EffectiveRoleId",
                schema: "dbo",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "LoginType",
                schema: "dbo",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "ParentId",
                schema: "dbo",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "SelectedStudentDetailsId",
                schema: "dbo",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "StudentUserId",
                schema: "dbo",
                table: "Tokens");
        }
    }
}
