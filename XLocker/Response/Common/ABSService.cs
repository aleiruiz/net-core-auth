using XLocker.Entities;
using XLocker.Types;

namespace XLocker.Response.Common
{
    public class ABSService : BaseEntity
    {

        internal string? UserName;

        internal string? UserEmail;

        internal string? UserPhoneNumber;

        public string CustomerToken { get; set; } = string.Empty;

        public string SupportToken { get; set; } = string.Empty;

        public int? Cost { get; set; }

        public string? Identifier { get; set; }

        public DateTime? DepositDate { get; set; }

        public DateTime? WithdrawalDate { get; set; }

        public DateTime? NoveltyDate { get; set; }

        public ServiceStatus Status { get; set; }

        public NoveltyType? NoveltyType { get; set; }

        public required string LockerId { get; set; }

        public string MailboxId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
    }
}
