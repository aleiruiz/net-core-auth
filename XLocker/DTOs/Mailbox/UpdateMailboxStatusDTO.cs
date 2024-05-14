using XLocker.Types;

namespace XLocker.DTOs.Mailbox
{
    public class UpdateMailboxStatusDTO
    {
        public MailboxStatus Status { get; set; } = MailboxStatus.Available;
    }
}
