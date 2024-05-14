using XLocker.Response.Common;
using XLocker.Response.Service;
using XLocker.Response.User;

namespace XLocker.Response.WithdrawalOrder
{
    public class WithdrawalOrderResponse : ABSWithdrawalOrder
    {
        public VerboseServiceResponse Service { get; set; } = null!;

        public RestrictedUser Courier { get; set; } = null!;
    }
}
