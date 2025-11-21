using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMIProjectAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountPaidToBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "Bill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Bill");
        }
    }
}

