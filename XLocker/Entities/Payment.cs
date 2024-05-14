using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XLocker.Types;

namespace XLocker.Entities
{
    public class Payment : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public string GuideNumber { get; set; } = string.Empty;

        public string GuideHash { get; set; } = string.Empty;

        public float Amount { get; set; }

        public int Credits { get; set; }

        public string? Concept { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string? CreditPackageId { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(24)")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public User User { get; set; } = null!;

        public CreditPackage CreditPackage { get; set; } = null!;

        internal string? UserName;

        internal string? CreditPackageName;
    }
}
