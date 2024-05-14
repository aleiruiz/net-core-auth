using XLocker.DTOs.Common;
using XLocker.Types;

namespace XLocker.DTOs.Mailbox
{
    public class GetMailboxDTO : QueryParamsDTO
    {

        public MailboxStatus? Status { get; set; }

        public MailboxSize? Size { get; set; }

        public MailboxPosition? Position { get; set; }

        public string? LockerId { get; set; }
    }
}
