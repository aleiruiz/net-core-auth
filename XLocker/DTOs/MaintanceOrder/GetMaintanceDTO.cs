using XLocker.DTOs.Common;
using XLocker.Types;

namespace XLocker.DTOs.MaintanceOrder
{
    public class GetMaintanceDTO : QueryParamsDTO
    {
        public MaintanceType? MaintanceType { get; set; }

        public string? LockerId { get; set; }
    }
}
