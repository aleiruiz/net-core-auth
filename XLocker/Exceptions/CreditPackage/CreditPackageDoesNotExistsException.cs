namespace XLocker.Exceptions.CreditPackage
{
    public class CreditPackageDoesNotExistsException : Exception
    {
        public CreditPackageDoesNotExistsException()
        {
            throw new CreditPackageDoesNotExistsException("Este paquete no existe");
        }

        public CreditPackageDoesNotExistsException(string message) : base(message)
        {
        }

        public CreditPackageDoesNotExistsException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
