namespace XLocker.Exceptions.Service
{
    public class MailboxNotAssignedException : Exception
    {
        public MailboxNotAssignedException()
        {
            throw new MailboxNotAssignedException("No se ha asignado una taquilla para este servicio");
        }

        public MailboxNotAssignedException(string message) : base(message)
        {
        }

        public MailboxNotAssignedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
