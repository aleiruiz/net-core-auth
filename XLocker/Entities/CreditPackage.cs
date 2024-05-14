using System.ComponentModel.DataAnnotations;

namespace XLocker.Entities
{
    public class CreditPackage : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public int CreditQuantity { get; set; }

        public float Price { get; set; }

        public string Name { get; set; } = string.Empty;

    }
}
