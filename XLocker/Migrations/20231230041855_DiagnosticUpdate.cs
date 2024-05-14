using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XLocker.Migrations
{
    /// <inheritdoc />
    public partial class DiagnosticUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Connection",
                table: "Lockers");

            migrationBuilder.DropColumn(
                name: "ConnectionState",
                table: "Diagnostics");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Diagnostics");

            migrationBuilder.DropColumn(
                name: "DiagnosticDate",
                table: "Diagnostics");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetRoles");

            migrationBuilder.RenameColumn(
                name: "LocksOpen",
                table: "Diagnostics",
                newName: "MailboxOpenQuantity");

            migrationBuilder.AddColumn<int>(
                name: "ConnectionState",
                table: "Lockers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastConnection",
                table: "Lockers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ClosedLocks",
                table: "Diagnostics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenLocks",
                table: "Diagnostics",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionState",
                table: "Lockers");

            migrationBuilder.DropColumn(
                name: "LastConnection",
                table: "Lockers");

            migrationBuilder.DropColumn(
                name: "ClosedLocks",
                table: "Diagnostics");

            migrationBuilder.DropColumn(
                name: "OpenLocks",
                table: "Diagnostics");

            migrationBuilder.RenameColumn(
                name: "MailboxOpenQuantity",
                table: "Diagnostics",
                newName: "LocksOpen");

            migrationBuilder.AddColumn<string>(
                name: "Connection",
                table: "Lockers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionState",
                table: "Diagnostics",
                type: "nvarchar(24)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Diagnostics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DiagnosticDate",
                table: "Diagnostics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetRoles",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");
        }
    }
}
