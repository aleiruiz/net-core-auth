namespace XLocker.Exceptions.User
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException()
        {
            throw new UserAlreadyExistException("Este email ya esta registrado");
        }

        public UserAlreadyExistException(string message) : base(message)
        {
        }

        public UserAlreadyExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
