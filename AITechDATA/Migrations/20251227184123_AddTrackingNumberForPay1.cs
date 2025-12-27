using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackingNumberForPay1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TrackingNumber",
                table: "PaymentHistories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "PaymentHistories");
        }
    }
}
