namespace XLocker.DTOs.Users
{
    public class UpdatePasswordDTO
    {
        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }
    }
}
