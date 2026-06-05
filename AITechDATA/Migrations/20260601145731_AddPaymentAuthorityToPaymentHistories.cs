using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAuthorityToPaymentHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentAuthority",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE PaymentHistories
                SET PaymentAuthority = JSON_VALUE(OtherLangs, '$.paymentGateway.authority')
                WHERE PaymentAuthority IS NULL
                  AND ISJSON(OtherLangs) = 1
                  AND JSON_VALUE(OtherLangs, '$.paymentGateway.authority') IS NOT NULL
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentAuthority",
                table: "PaymentHistories");
        }
    }
}
