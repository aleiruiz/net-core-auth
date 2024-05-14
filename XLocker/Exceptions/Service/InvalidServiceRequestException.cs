namespace XLocker.Exceptions.Service
{
    public class InvalidServiceRequestException : Exception
    {
        public InvalidServiceRequestException()
        {
            throw new InvalidServiceRequestException("Esta peticion es invalida");
        }

        public InvalidServiceRequestException(string message) : base(message)
        {
        }

        public InvalidServiceRequestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
