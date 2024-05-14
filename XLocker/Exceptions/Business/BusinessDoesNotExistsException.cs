namespace XLocker.Exceptions.Business
{
    public class BusinessDoesNotExistsException : Exception
    {
        public BusinessDoesNotExistsException()
        {
            throw new BusinessDoesNotExistsException("Este negocio no existe");
        }

        public BusinessDoesNotExistsException(string message) : base(message)
        {
        }

        public BusinessDoesNotExistsException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
