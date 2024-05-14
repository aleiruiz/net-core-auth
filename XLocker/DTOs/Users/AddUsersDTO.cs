using XLocker.Types;

namespace XLocker.DTOs.Users
{
    public class AddUsersDTO
    {
        public required string Email { get; set; }

        public required string Name { get; set; }

        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Inactive;

        public required string[] roles { get; set; }
    }
}
