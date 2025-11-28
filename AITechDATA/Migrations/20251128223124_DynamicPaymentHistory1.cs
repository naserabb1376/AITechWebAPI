using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class DynamicPaymentHistory1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentHistories_Groups_GroupId",
                table: "PaymentHistories");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "PaymentHistories",
                newName: "GroupID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentHistories_GroupId",
                table: "PaymentHistories",
                newName: "IX_PaymentHistories_GroupID");

            migrationBuilder.AlterColumn<long>(
                name: "GroupID",
                table: "PaymentHistories",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ForeignKeyId",
                table: "PaymentHistories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "Events",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentHistories_Groups_GroupID",
                table: "PaymentHistories",
                column: "GroupID",
                principalTable: "Groups",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentHistories_Groups_GroupID",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "ForeignKeyId",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "GroupID",
                table: "PaymentHistories",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentHistories_GroupID",
                table: "PaymentHistories",
                newName: "IX_PaymentHistories_GroupId");

            migrationBuilder.AlterColumn<long>(
                name: "GroupId",
                table: "PaymentHistories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentHistories_Groups_GroupId",
                table: "PaymentHistories",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
