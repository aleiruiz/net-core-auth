namespace XLocker.Exceptions.User
{
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException()
        {
            throw new UserDoesNotExistException("Este usuario no existe");
        }

        public UserDoesNotExistException(string message) : base(message)
        {
        }

        public UserDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
