using System.ComponentModel.DataAnnotations;

namespace XLocker.Entities
{
    public class EmailTemplate : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Name { get; set; }

        public string Template { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;
    }
}
