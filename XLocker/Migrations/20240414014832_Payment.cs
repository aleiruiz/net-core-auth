using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XLocker.Migrations
{
    /// <inheritdoc />
    public partial class Payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreditPackageId",
                table: "Payments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreditPackageId",
                table: "Payments",
                column: "CreditPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_CreditPackages_CreditPackageId",
                table: "Payments",
                column: "CreditPackageId",
                principalTable: "CreditPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_CreditPackages_CreditPackageId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CreditPackageId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreditPackageId",
                table: "Payments");
        }
    }
}
