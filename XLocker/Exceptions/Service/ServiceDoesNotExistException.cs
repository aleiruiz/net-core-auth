namespace XLocker.Exceptions.Service
{
    public class ServiceDoesNotExistException : Exception
    {
        public ServiceDoesNotExistException()
        {
            throw new ServiceDoesNotExistException("Este servicio no existe");
        }

        public ServiceDoesNotExistException(string message) : base(message)
        {
        }

        public ServiceDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
