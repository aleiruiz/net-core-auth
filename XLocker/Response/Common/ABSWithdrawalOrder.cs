using XLocker.Entities;
using XLocker.Types;

namespace XLocker.Response.Common
{
    public class ABSWithdrawalOrder : BaseEntity
    {
        public required string ServiceId { get; set; }

        public string? UserId { get; set; }

        public required ServiceStatus ServiceStatus { get; set; }

        public DateTime? WithdrawlDate { get; set; }
    }
}
