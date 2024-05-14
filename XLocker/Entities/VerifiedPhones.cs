using System.ComponentModel.DataAnnotations;

namespace XLocker.Entities
{
    public class VerifiedPhones : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public string PhoneNumber { get; set; } = string.Empty;

    }
}
