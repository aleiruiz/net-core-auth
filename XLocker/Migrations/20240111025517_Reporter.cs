using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XLocker.Migrations
{
    /// <inheritdoc />
    public partial class Reporter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReporterId",
                table: "MaintanceOders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintanceOders_ReporterId",
                table: "MaintanceOders",
                column: "ReporterId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintanceOders_AspNetUsers_ReporterId",
                table: "MaintanceOders",
                column: "ReporterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintanceOders_AspNetUsers_ReporterId",
                table: "MaintanceOders");

            migrationBuilder.DropIndex(
                name: "IX_MaintanceOders_ReporterId",
                table: "MaintanceOders");

            migrationBuilder.DropColumn(
                name: "ReporterId",
                table: "MaintanceOders");
        }
    }
}
