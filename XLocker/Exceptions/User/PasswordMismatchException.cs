namespace XLocker.Exceptions.User
{
    public class PasswordMismatchException : Exception
    {
        public PasswordMismatchException()
        {
            throw new PasswordMismatchException("Las contraseñas deben coincidir");
        }

        public PasswordMismatchException(string message) : base(message)
        {
        }

        public PasswordMismatchException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
