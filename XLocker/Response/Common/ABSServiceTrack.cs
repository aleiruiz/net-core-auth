using XLocker.Entities;

namespace XLocker.Response.Common
{
    public class ABSServiceTrack : BaseEntity
    {
        public required string Image { get; set; }

        public required string ServiceId { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
