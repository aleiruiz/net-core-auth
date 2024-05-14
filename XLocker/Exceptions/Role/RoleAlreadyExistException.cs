namespace XLocker.Exceptions.Role
{
    public class RoleAlreadyExistException : Exception
    {
        public RoleAlreadyExistException()
        {
            throw new RoleAlreadyExistException("Este rol ya existe");
        }

        public RoleAlreadyExistException(string message) : base(message)
        {
        }

        public RoleAlreadyExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
