namespace XLocker.DTOs.Roles
{
    public class UpdateRoleDTO
    {
        public required string Name { get; set; }

        public required string[] Permissions { get; set; }
    }
}
