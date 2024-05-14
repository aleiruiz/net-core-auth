using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XLocker.Migrations
{
    /// <inheritdoc />
    public partial class Maintance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintanceOders_Lockers_LockerId",
                table: "MaintanceOders");

            migrationBuilder.AlterColumn<string>(
                name: "LockerId",
                table: "MaintanceOders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MaintanceOders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MaintanceOders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintanceOders_UserId",
                table: "MaintanceOders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintanceOders_AspNetUsers_UserId",
                table: "MaintanceOders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintanceOders_Lockers_LockerId",
                table: "MaintanceOders",
                column: "LockerId",
                principalTable: "Lockers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintanceOders_AspNetUsers_UserId",
                table: "MaintanceOders");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintanceOders_Lockers_LockerId",
                table: "MaintanceOders");

            migrationBuilder.DropIndex(
                name: "IX_MaintanceOders_UserId",
                table: "MaintanceOders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MaintanceOders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MaintanceOders");

            migrationBuilder.AlterColumn<string>(
                name: "LockerId",
                table: "MaintanceOders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintanceOders_Lockers_LockerId",
                table: "MaintanceOders",
                column: "LockerId",
                principalTable: "Lockers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
