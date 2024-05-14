using XLocker.Entities;

namespace XLocker.Response.Common
{
    public class ABSPayment : BaseEntity
    {
        public string GuideNumber { get; set; } = string.Empty;

        public string GuideHash { get; set; } = string.Empty;

        public float Amount { get; set; }

        public float Credits { get; set; }

        public string? Concept { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string IdentityKey { get; set; } = string.Empty;
    }
}
