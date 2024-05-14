using XLocker.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XLocker.Entities
{
    public class Locker : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public required string Name { get; set; }

        public string Reference { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int MaxMailboxes { get; set; } = 0;

        public ConnectionState ConnectionState { get; set; } = ConnectionState.Online;

        public DateTime LastConnection { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "nvarchar(24)")]
        public LockerStatus? Status { get; set; } = LockerStatus.Enabled;

        public ICollection<Mailbox> Mailboxes { get; } = new List<Mailbox>();

        public ICollection<Service> Services { get; } = new List<Service>();

        [NotMapped]
        public int MailboxQuantity { get; set; }

        [NotMapped]
        public int MailboxAvailableQuantity { get; set; }
    }
}
