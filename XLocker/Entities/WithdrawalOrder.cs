using System.ComponentModel.DataAnnotations;
using XLocker.Types;

namespace XLocker.Entities
{
    public class WithdrawalOrder : BaseEntity
    {

        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public required string ServiceId { get; set; }

        public string? UserId { get; set; }

        public required ServiceStatus ServiceStatus { get; set; }

        public DateTime? WithdrawlDate { get; set; }

        public Service Service { get; set; } = null!;

        public User User { get; set; } = null!;

        internal string? ServiceLockerName;

        internal string? ServiceMailboxNumber;

        internal string? UserName;

        internal string? UserEmail;

    }
}
