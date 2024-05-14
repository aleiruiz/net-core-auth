using XLocker.Types;

namespace XLocker.Response.Service
{
    public class ServiceExcel
    {
        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? UserPhoneNumber { get; set; }

        public int? Cost { get; set; }

        public string? LockerName { get; set; }

        public string? MailboxNumber { get; set; }

        public DateTime? DepositDate { get; set; }

        public DateTime? WithdrawalDate { get; set; }

        public DateTime? NoveltyDate { get; set; }

        public ServiceStatus Status { get; set; }

        public NoveltyType? NoveltyType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
