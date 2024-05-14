namespace XLocker.DTOs.Diagnostic
{
    public class CreateDiagnosticDTO
    {
        public int MailboxQuantity { get; set; }

        public int MailboxOpenQuantity { get; set; }

        public string[]? ClosedLocks { get; set; }

        public string[]? OpenLocks { get; set; }

        public required string LockerId { get; set; }
    }
}
