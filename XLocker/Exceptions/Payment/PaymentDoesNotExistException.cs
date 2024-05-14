namespace XLocker.Exceptions.CreditPackage
{
    public class PaymentDoesNotExistException : Exception
    {
        public PaymentDoesNotExistException()
        {
            throw new PaymentDoesNotExistException("Este pago no existe");
        }

        public PaymentDoesNotExistException(string message) : base(message)
        {
        }

        public PaymentDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
