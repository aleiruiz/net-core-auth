using XLocker.DTOs.Common;
using XLocker.Types;

namespace XLocker.DTOs.Service
{
    public class GetServiceDTO : QueryParamsDTO
    {
        public ServiceStatus? Status { get; set; }
    }
}
