namespace XLocker.Exceptions.Business
{
    public class BusinessAlreadyExistException : Exception
    {
        public BusinessAlreadyExistException()
        {
            throw new BusinessAlreadyExistException("Esta empresa ya esta registrada");
        }

        public BusinessAlreadyExistException(string message) : base(message)
        {
        }

        public BusinessAlreadyExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
