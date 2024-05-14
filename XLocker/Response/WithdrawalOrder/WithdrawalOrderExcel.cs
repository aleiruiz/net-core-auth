using XLocker.Types;

namespace XLocker.Response.WithdrawalOrder
{
    public class WithdrawalOrderExcel
    {
        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? ServiceLockerName { get; set; }

        public string? ServiceMailboxNumber { get; set; }

        public required ServiceStatus ServiceStatus { get; set; }

        public DateTime? WithdrawlDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
