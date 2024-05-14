namespace XLocker.DTOs.Auth
{
    public class RegisterDTO
    {
        public required string Name { get; set; } = string.Empty;

        public required string PhoneNumber { get; set; } = string.Empty;

        public required string Email { get; set; } = string.Empty;

        public required string Password { get; set; } = string.Empty;

        public required string ConfirmPassword { get; set; }
    }
}
