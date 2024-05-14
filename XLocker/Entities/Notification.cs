using System.ComponentModel.DataAnnotations;

namespace XLocker.Entities
{
    public class Notification : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Description { get; set; }

        public required string Title { get; set; }
    }
}
