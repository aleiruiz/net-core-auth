namespace XLocker.Exceptions.WithdrawalOrder
{
    public class WithdrawalOrderAlreadyExist : Exception
    {
        public WithdrawalOrderAlreadyExist()
        {
            throw new WithdrawalOrderAlreadyExist("Ya existe una peticion de retiro para este servicio");
        }

        public WithdrawalOrderAlreadyExist(string message) : base(message)
        {
        }

        public WithdrawalOrderAlreadyExist(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
