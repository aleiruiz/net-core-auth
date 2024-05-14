using XLocker.Entities;

namespace XLocker.Response.Common
{
    public class ABSLocker : BaseEntity
    {
        public required string Name { get; set; }

        public string Reference { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string ConnectionState { get; set; } = string.Empty;

        public DateTime LastConnection { get; set; }

        public string Status { get; set; } = string.Empty;

        public int MaxMailboxes { get; set; }

        public int MailboxQuantity { get; set; }

        public int MailboxAvailableQuantity { get; set; }
    }
}
