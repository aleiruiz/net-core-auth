namespace XLocker.Exceptions.Role
{
    public class NotificationDoesNotExistException : Exception
    {
        public NotificationDoesNotExistException()
        {
            throw new NotificationDoesNotExistException("Esta notificacion no existe");
        }

        public NotificationDoesNotExistException(string message) : base(message)
        {
        }

        public NotificationDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
