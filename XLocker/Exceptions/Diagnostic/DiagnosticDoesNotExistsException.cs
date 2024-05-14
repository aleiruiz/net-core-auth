namespace XLocker.Exceptions.Diagnostic
{
    public class DiagnosticDoesNotExistsException : Exception
    {
        public DiagnosticDoesNotExistsException()
        {
            throw new DiagnosticDoesNotExistsException("Este casillero no existe");
        }

        public DiagnosticDoesNotExistsException(string message) : base(message)
        {
        }

        public DiagnosticDoesNotExistsException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
