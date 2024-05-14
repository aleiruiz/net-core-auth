namespace XLocker.Exceptions.Locker
{
    public class LockerAlreadyExistException : Exception
    {
        public LockerAlreadyExistException()
        {
            throw new LockerAlreadyExistException("El nombre de este casillero ya existe");
        }

        public LockerAlreadyExistException(string message) : base(message)
        {
        }

        public LockerAlreadyExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
