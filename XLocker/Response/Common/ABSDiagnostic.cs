using XLocker.Entities;

namespace XLocker.Response.Maintance
{
    public class ABSDiagnostic : BaseEntity
    {
        public int MailboxQuantity { get; set; }

        public int MailboxOpenQuantity { get; set; }

        public string? ClosedLocks { get; set; }

        public string? OpenLocks { get; set; }

        public required string LockerId { get; set; }
    }
}
