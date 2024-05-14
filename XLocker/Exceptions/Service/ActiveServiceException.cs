namespace XLocker.Exceptions.Service
{
    public class ActiveServiceException : Exception
    {
        public ActiveServiceException()
        {
            throw new ActiveServiceException("Ya tienes un servicio activo");
        }

        public ActiveServiceException(string message) : base(message)
        {
        }

        public ActiveServiceException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
