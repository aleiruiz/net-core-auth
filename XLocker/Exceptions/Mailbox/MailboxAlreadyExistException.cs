namespace XLocker.Exceptions.Mailbox
{
    public class MailboxAlreadyExistException : Exception
    {
        public MailboxAlreadyExistException()
        {
            throw new MailboxAlreadyExistException("Este numero de taquilla ya existe en este casillero");
        }

        public MailboxAlreadyExistException(string message) : base(message)
        {
        }

        public MailboxAlreadyExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
