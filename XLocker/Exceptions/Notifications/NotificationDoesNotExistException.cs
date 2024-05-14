namespace XLocker.Exceptions.Role
{
    public class RoleDoesNotExistException : Exception
    {
        public RoleDoesNotExistException()
        {
            throw new RoleDoesNotExistException("Este rol no existe");
        }

        public RoleDoesNotExistException(string message) : base(message)
        {
        }

        public RoleDoesNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
