using XLocker.Types;

namespace XLocker.DTOs.MaintanceOrder
{
    public class UpdateMaintanceDTO
    {
        public string Description { get; set; } = string.Empty;

        public required DateTime MaintanceDate { get; set; }

        public required MaintanceType MaintanceType { get; set; }

        public required string LockerId { get; set; }
    }
}
