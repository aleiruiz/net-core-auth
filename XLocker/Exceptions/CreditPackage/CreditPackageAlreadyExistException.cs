namespace XLocker.Exceptions.CreditPackage
{
    public class CreditPackageAlreadyExistException : Exception
    {
        public CreditPackageAlreadyExistException()
        {
            throw new CreditPackageAlreadyExistException("Este paquete ya existe");
        }

        public CreditPackageAlreadyExistException(string message) : base(message)
        {
        }

        public CreditPackageAlreadyExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
