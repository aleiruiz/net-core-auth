using XLocker.Emails;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XLocker.Migrations
{
    /// <inheritdoc />
    public partial class EmailTemplateSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "Id", "Name", "Template", "Subject", "Note", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid().ToString(), "Deposit", DepositEmail.Template, DepositEmail.Subject, DepositEmail.Note, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid().ToString(), "Reminder", ReminderEmail.Template, ReminderEmail.Subject, ReminderEmail.Note, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid().ToString(), "UrgentReminder", UrgentReminderEmail.Template, UrgentReminderEmail.Subject, UrgentReminderEmail.Note, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid().ToString(), "DueService", DueServiceEmail.Template, DueServiceEmail.Subject, DueServiceEmail.Note, DateTime.UtcNow, DateTime.UtcNow },
                    { Guid.NewGuid().ToString(), "Withdrawl", WithdrawlEmail.Template, WithdrawlEmail.Subject, WithdrawlEmail.Note, DateTime.UtcNow, DateTime.UtcNow },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
