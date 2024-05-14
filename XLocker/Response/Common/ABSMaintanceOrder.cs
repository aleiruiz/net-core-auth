using XLocker.Entities;

namespace XLocker.Response.Common
{
    public class ABSMaintanceOrder : BaseEntity
    {
        public string Description { get; set; } = string.Empty;

        public required string Status { get; set; }

        public required DateTime MaintanceDate { get; set; }

        public required string MaintanceType { get; set; }

        public required string LockerId { get; set; }

        public required string UserId { get; set; }
    }
}
