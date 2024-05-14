using XLocker.Entities;

namespace XLocker.Response.Common
{
    public class ABSRole : BaseEntity
    {
        public ICollection<string>? Permissions { get; set; }

        public string? Name { get; set; }
    }
}
