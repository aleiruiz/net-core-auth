namespace XLocker.Exceptions.Auth
{
    public class AdminOnlyException : Exception
    {
        public AdminOnlyException()
        {
            throw new InvalidPasswordException("Acceso restringido, solo administradores");
        }

        public AdminOnlyException(string message) : base(message)
        {
        }

        public AdminOnlyException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
