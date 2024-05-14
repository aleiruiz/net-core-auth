using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XLocker.Migrations
{
    /// <inheritdoc />
    public partial class ServicesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "Services",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UrgentReminderSent",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ParentId",
                table: "Services",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Services_ParentId",
                table: "Services",
                column: "ParentId",
                principalTable: "Services",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Services_ParentId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ParentId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UrgentReminderSent",
                table: "Services");
        }
    }
}
