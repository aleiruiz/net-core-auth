namespace XLocker.Exceptions.User
{
    public class PhoneNotFoundException : Exception
    {
        public PhoneNotFoundException()
        {
            throw new PhoneNotFoundException("Este usuario no tiene un telefono registrado, porfavor, proporciona uno para autenticarlo");
        }

        public PhoneNotFoundException(string message) : base(message)
        {
        }

        public PhoneNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
