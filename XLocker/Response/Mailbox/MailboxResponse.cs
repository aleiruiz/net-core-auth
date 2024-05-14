using XLocker.Response.Common;

namespace XLocker.Response.Mailbox
{
    public class MailboxResponse : ABSMailbox
    {
        public ABSLocker Locker { get; set; } = null!;
    }
}
