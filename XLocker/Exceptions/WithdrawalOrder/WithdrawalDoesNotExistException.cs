namespace XLocker.Exceptions.WithdrawalOrder
{
    public class WithdrawalDoesNotExistException : Exception
    {
        public WithdrawalDoesNotExistException()
        {
            throw new WithdrawalDoesNotExistException("Este servicio no existe");
        }

        public WithdrawalDoesNotExistException(string message) : base(message)
        {
        }

        public WithdrawalDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
