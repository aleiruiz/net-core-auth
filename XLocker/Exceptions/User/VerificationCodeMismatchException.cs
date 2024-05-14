namespace XLocker.Exceptions.User
{
    public class VerificationCodeMismatchException : Exception
    {
        public VerificationCodeMismatchException()
        {
            throw new VerificationCodeMismatchException("El codigo de verificacion no coincide con el registro");
        }

        public VerificationCodeMismatchException(string message) : base(message)
        {
        }

        public VerificationCodeMismatchException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
