using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class addFieldsToJobRequest3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "PreRegistrations");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "JobRequests"
                );

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "PreRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
               name: "LastName",
               table: "PreRegistrations",
               type: "nvarchar(max)",
               nullable: false,
               defaultValue: "");

            migrationBuilder.AddColumn<string>(
            name: "FirstName",
            table: "JobRequests",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

            migrationBuilder.AddColumn<string>(
               name: "LastName",
               table: "JobRequests",
               type: "nvarchar(max)",
               nullable: false,
               defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "JobRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationStatus",
                table: "JobRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationalLevel",
                table: "JobRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "JobRequests",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "FirstName",
            //    table: "JobRequests",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastAcademicLicense",
                table: "JobRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalCode",
                table: "JobRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniversityName",
                table: "JobRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "PreRegistrations");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "JobRequests");

            migrationBuilder.DropColumn(
                name: "EducationStatus",
                table: "JobRequests");

            migrationBuilder.DropColumn(
                name: "EducationalLevel",
                table: "JobRequests");

            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "JobRequests");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "JobRequests");

            migrationBuilder.DropColumn(
                name: "LastAcademicLicense",
                table: "JobRequests");

            migrationBuilder.DropColumn(
                name: "NationalCode",
                table: "JobRequests");

            migrationBuilder.DropColumn(
                name: "UniversityName",
                table: "JobRequests");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "PreRegistrations",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "JobRequests",
                newName: "FullName");
        }
    }
}
