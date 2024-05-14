using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XLocker.Types;

namespace XLocker.Entities
{
    public class Service : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public string CustomerToken { get; set; } = string.Empty;

        public string? SupportToken { get; set; }

        public DateTime? DepositDate { get; set; }

        public DateTime? WithdrawalDate { get; set; }

        public DateTime? NoveltyDate { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        public required ServiceStatus Status { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        public NoveltyType? NoveltyType { get; set; }

        public required string LockerId { get; set; }

        public bool ReminderSent { get; set; } = false;

        public bool UrgentReminderSent { get; set; } = false;

        public bool WithdrawlRequested { get; set; } = false;

        public string? MailboxId { get; set; }

        public string UserId { get; set; }

        public string? QRCode { get; set; }

        public Locker Locker { get; set; } = null!;

        public Mailbox Mailbox { get; set; } = null!;

        public int? Cost { get; set; }

        public string? Identifier { get; set; }

        public User User { get; set; } = null!;

        public ICollection<ServiceTrack> ServiceTracks { get; } = new List<ServiceTrack>();

        internal string? LockerName;

        internal string? MailboxNumber;

        internal string? UserName;

        internal string? UserEmail;

        internal string? UserPhoneNumber;
    }
}
