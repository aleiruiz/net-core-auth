namespace XLocker.Exceptions.Mailbox
{
    public class MailboxDoesNotExistException : Exception
    {
        public MailboxDoesNotExistException()
        {
            throw new MailboxDoesNotExistException("Esta taquilla no existe");
        }

        public MailboxDoesNotExistException(string message) : base(message)
        {
        }

        public MailboxDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
