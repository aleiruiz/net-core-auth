namespace XLocker.Exceptions.Auth
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException()
        {
            throw new InvalidPasswordException("La contraseña es invalida");
        }

        public InvalidPasswordException(string message) : base(message)
        {
        }

        public InvalidPasswordException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
