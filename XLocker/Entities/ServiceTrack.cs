using System.ComponentModel.DataAnnotations;

namespace XLocker.Entities
{
    public class ServiceTrack : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Image { get; set; }

        public required string ServiceId { get; set; }

        public string? Description { get; set; }

        public Service Service { get; set; } = null!;
    }
}
