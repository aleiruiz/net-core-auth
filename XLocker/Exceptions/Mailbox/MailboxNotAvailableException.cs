namespace XLocker.Exceptions.Mailbox
{
    public class MailboxNotAvailableException : Exception
    {
        public MailboxNotAvailableException()
        {
            throw new MailboxNotAvailableException("Esta taquilla esta ocupada");
        }

        public MailboxNotAvailableException(string message) : base(message)
        {
        }

        public MailboxNotAvailableException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
