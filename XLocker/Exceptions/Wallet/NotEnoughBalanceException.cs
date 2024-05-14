namespace XLocker.Exceptions.User
{
    public class NotEnoughBalanceException : Exception
    {
        public NotEnoughBalanceException()
        {
            throw new NotEnoughBalanceException("No tiene suficiente saldo para completar la transaccion");
        }

        public NotEnoughBalanceException(string message) : base(message)
        {
        }

        public NotEnoughBalanceException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
