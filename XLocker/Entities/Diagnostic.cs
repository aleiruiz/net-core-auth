using System.ComponentModel.DataAnnotations;

namespace XLocker.Entities
{
    public class Diagnostic : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public int MailboxQuantity { get; set; }

        public int MailboxOpenQuantity { get; set; }

        public string? ClosedLocks { get; set; }

        public string? OpenLocks { get; set; }

        public required string LockerId { get; set; }

        public Locker Locker { get; set; } = null!;

    }
}
