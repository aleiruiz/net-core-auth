using XLocker.Entities;

namespace XLocker.Response.Common
{
    public class ABSUser : BaseEntity
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; } = string.Empty;

        public bool PhoneNumberConfirmed { get; set; }

        public bool RewardClaimed { get; set; }

        public string? Password { get; set; }

        public required string Status { get; set; }

        public string? Role { get; set; }

        public List<string>? Permissions { get; set; }

        public float? Balance { get; set; }

        public ICollection<ABSRole>? Roles { get; set; }
    }
}
