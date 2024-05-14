using XLocker.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XLocker.Entities
{
    public class MaintanceOrder : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public string Description { get; set; } = string.Empty;

        public MaintanceStatus Status { get; set; } = MaintanceStatus.MC;

        public required DateTime MaintanceDate { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        public required MaintanceType MaintanceType { get; set; }

        public string? LockerId { get; set; }

        public string? UserId { get; set; }

        public string? ReporterId { get; set; }

        public Locker? Locker { get; set; } = null!;

        public User? User { get; set; } = null!;

        public User? Reporter { get; set; }

    }
}
