using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionCodeToPaymentHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionCode",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE PaymentHistories
                SET TransactionCode = JSON_VALUE(OtherLangs, '$.paymentGateway.transactionCode')
                WHERE TransactionCode IS NULL
                  AND ISJSON(OtherLangs) = 1
                  AND JSON_VALUE(OtherLangs, '$.paymentGateway.transactionCode') IS NOT NULL
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionCode",
                table: "PaymentHistories");
        }
    }
}
