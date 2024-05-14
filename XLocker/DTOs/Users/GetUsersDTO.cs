using XLocker.DTOs.Common;

namespace XLocker.DTOs.Users
{
    public class GetRestrictedUserDTO : QueryParamsDTO
    {
        public string? Roles { get; set; }
    }
}
