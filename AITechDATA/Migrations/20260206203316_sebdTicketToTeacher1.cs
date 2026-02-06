using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class sebdTicketToTeacher1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TeacherId",
                table: "Tickets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ResponserRoleId",
                table: "TicketMessages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialAddress",
                table: "PreRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TeacherId",
                table: "Tickets",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_TeacherId",
                table: "Tickets",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_TeacherId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TeacherId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ResponserRoleId",
                table: "TicketMessages");

            migrationBuilder.DropColumn(
                name: "SocialAddress",
                table: "PreRegistrations");
        }
    }
}
