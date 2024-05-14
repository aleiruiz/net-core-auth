using XLocker.Types;

namespace XLocker.DTOs.Users
{
    public class UpdateUsersDTO
    {
        public required string Email { get; set; }

        public required string Name { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Inactive;

        public required string[] roles { get; set; }
    }
}
