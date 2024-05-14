using XLocker.DTOs.Common;
using XLocker.Types;

namespace XLocker.DTOs.Locker
{
    public class GetLockerDTO : QueryParamsDTO
    {
        public LockerStatus? Status { get; set; }
    }
}
