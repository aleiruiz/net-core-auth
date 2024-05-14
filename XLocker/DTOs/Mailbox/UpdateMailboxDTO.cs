using XLocker.Types;

namespace XLocker.DTOs.Mailbox
{
    public class UpdateMailboxDTO
    {
        public required string Number { get; set; }

        public MailboxStatus Status { get; set; } = MailboxStatus.Available;

        public MailboxSize Size { get; set; } = MailboxSize.SM;

        public MailboxPosition Position { get; set; } = MailboxPosition.Open;

        public required string LockerId { get; set; }
    }
}
