namespace XLocker.Exceptions.Service
{
    public class ServiceWrongStepException : Exception
    {
        public ServiceWrongStepException()
        {
            throw new ServiceWrongStepException("No se puede realizar esta accion en este servicio ya que no es el paso correspondiente.");
        }

        public ServiceWrongStepException(string message) : base(message)
        {
        }

        public ServiceWrongStepException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
