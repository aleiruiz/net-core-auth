using XLocker.Response.Common;

namespace XLocker.Response.Maintance
{
    public class MaintanceResponse : ABSMaintanceOrder
    {
        public ABSLocker Locker { get; set; } = null!;
        public ABSUser User { get; set; } = null!;
        public ABSUser Reporter { get; set; } = null!;
    }
}
