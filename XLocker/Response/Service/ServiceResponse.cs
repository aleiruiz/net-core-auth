using XLocker.Response.Common;
using XLocker.Response.User;

namespace XLocker.Response.Service
{
    public class ServiceResponse : ABSService
    {
        public new string SupportToken { get { return "REDACTED"; } }

        public string? WithdrawalOrderId = string.Empty;

        public ABSLocker Locker { get; set; } = null!;

        public ABSMailbox Mailbox { get; set; } = null!;

        public RestrictedUser User { get; set; } = null!;

        public List<ABSServiceTrack> ServiceTracks { get; set; } = null!;
    }
}
