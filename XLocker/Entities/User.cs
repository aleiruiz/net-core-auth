using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using XLocker.Types;

namespace XLocker.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string PhoneVerificationCode { get; set; } = string.Empty;

        public bool RewardClaimed { get; set; }

        public bool IsAdmin { get; set; } = false;

        [Column(TypeName = "nvarchar(24)")]
        public virtual UserStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
