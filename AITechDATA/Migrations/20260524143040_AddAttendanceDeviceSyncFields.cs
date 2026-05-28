using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceDeviceSyncFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeFunctions_UserId",
                table: "TimeFunctions");

            migrationBuilder.AddColumn<string>(
                name: "AttendanceDeviceUserId",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SourceDeviceDate",
                table: "TimeFunctions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceDeviceEndLogKey",
                table: "TimeFunctions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceDeviceSerial",
                table: "TimeFunctions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceDeviceStartLogKey",
                table: "TimeFunctions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AttendanceDeviceUserId",
                table: "Users",
                column: "AttendanceDeviceUserId",
                unique: true,
                filter: "[AttendanceDeviceUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TimeFunctions_UserId_SourceDeviceSerial_SourceDeviceDate",
                table: "TimeFunctions",
                columns: new[] { "UserId", "SourceDeviceSerial", "SourceDeviceDate" },
                unique: true,
                filter: "[SourceDeviceSerial] IS NOT NULL AND [SourceDeviceDate] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AttendanceDeviceUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TimeFunctions_UserId_SourceDeviceSerial_SourceDeviceDate",
                table: "TimeFunctions");

            migrationBuilder.DropColumn(
                name: "AttendanceDeviceUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SourceDeviceDate",
                table: "TimeFunctions");

            migrationBuilder.DropColumn(
                name: "SourceDeviceEndLogKey",
                table: "TimeFunctions");

            migrationBuilder.DropColumn(
                name: "SourceDeviceSerial",
                table: "TimeFunctions");

            migrationBuilder.DropColumn(
                name: "SourceDeviceStartLogKey",
                table: "TimeFunctions");

            migrationBuilder.CreateIndex(
                name: "IX_TimeFunctions_UserId",
                table: "TimeFunctions",
                column: "UserId");
        }
    }
}
