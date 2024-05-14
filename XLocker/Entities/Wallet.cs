using System.ComponentModel.DataAnnotations;
using XLocker.Types;

namespace XLocker.Entities
{
    public class Wallet : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public string? UserId { get; set; }

        public int Credits { get; set; }

        public string? Concept { get; set; }

        public TransactionType TransactionType { get; set; }

        public User User { get; set; } = null!;

        internal string? UserName;
    }
}
