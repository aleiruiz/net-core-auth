using XLocker.Entities;

namespace XLocker.Response.Common
{
    public class ABSMailbox : BaseEntity
    {
        public required string Number { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Size { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public required string LockerId { get; set; }
    }
}
