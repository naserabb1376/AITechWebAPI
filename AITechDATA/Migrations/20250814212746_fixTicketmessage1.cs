using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class fixTicketmessage1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketMessages_Users_AdminId",
                table: "TicketMessages");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "TicketMessages",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketMessages_AdminId",
                table: "TicketMessages",
                newName: "IX_TicketMessages_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketMessages_Users_UserId",
                table: "TicketMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketMessages_Users_UserId",
                table: "TicketMessages");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TicketMessages",
                newName: "AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketMessages_UserId",
                table: "TicketMessages",
                newName: "IX_TicketMessages_AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketMessages_Users_AdminId",
                table: "TicketMessages",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "ID");
        }
    }
}
