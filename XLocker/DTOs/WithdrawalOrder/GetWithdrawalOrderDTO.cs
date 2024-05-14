using XLocker.DTOs.Common;
using XLocker.Types;

namespace XLocker.DTOs.WithdrawalOrder
{
    public class GetWithdrawalOrderDTO : QueryParamsDTO
    {
        public ServiceStatus? Status { get; set; }
    }
}
