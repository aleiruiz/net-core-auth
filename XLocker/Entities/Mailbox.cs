using XLocker.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XLocker.Entities
{
    public class Mailbox : BaseEntity
    {
        [Key]
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public required string Number { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        public MailboxStatus Status { get; set; } = MailboxStatus.Available;

        [Column(TypeName = "nvarchar(24)")]
        public MailboxSize Size { get; set; } = MailboxSize.SM;

        [Column(TypeName = "nvarchar(24)")]
        public MailboxPosition Position { get; set; } = MailboxPosition.Open;

        public required string LockerId { get; set; }

        public Locker Locker { get; set; } = null!;
    }
}
