namespace XLocker.Exceptions.Maintance
{
    public class MaintanceOrderDoesNotExistException : Exception
    {
        public MaintanceOrderDoesNotExistException()
        {
            throw new MaintanceOrderDoesNotExistException("Esta orden de mantenimiento no existe");
        }

        public MaintanceOrderDoesNotExistException(string message) : base(message)
        {
        }

        public MaintanceOrderDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
