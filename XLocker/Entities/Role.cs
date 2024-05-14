using Microsoft.AspNetCore.Identity;

namespace XLocker.Entities
{
    public class Role : IdentityRole
    {
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

        public Role()
        {
        }

        public Role(string Name)
        {
            this.Name = Name;
        }
    }
}
