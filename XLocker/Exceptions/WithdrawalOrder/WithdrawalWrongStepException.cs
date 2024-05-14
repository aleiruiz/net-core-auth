namespace XLocker.Exceptions.WithdrawalOrder
{
    public class WithdrawalWrongStepException : Exception
    {
        public WithdrawalWrongStepException()
        {
            throw new WithdrawalWrongStepException("No se puede realizar esta accion en esta solicitud de retiro ya que no es el paso correspondiente.");
        }

        public WithdrawalWrongStepException(string message) : base(message)
        {
        }

        public WithdrawalWrongStepException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
