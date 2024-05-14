namespace XLocker.DTOs.Auth
{
    public class ChangePasswordDTO
    {
        public required string CurrentPassword { get; set; }

        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }
    }
}
