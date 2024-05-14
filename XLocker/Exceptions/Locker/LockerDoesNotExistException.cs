namespace XLocker.Exceptions.Locker
{
    public class LockerDoesNotExistException : Exception
    {
        public LockerDoesNotExistException()
        {
            throw new LockerDoesNotExistException("Este casillero no existe");
        }

        public LockerDoesNotExistException(string message) : base(message)
        {
        }

        public LockerDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
