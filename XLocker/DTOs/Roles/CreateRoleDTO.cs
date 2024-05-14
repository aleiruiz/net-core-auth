namespace XLocker.DTOs.Roles
{
    public class CreateRoleDTO
    {
        public required string Name { get; set; }

        public required string[] Permissions { get; set; }
    }
}
