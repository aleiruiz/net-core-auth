using XLocker.Entities;
using XLocker.Response.Common;

namespace XLocker.Response.Payment
{
    public class PaymentResponse : ABSPayment
    {
        public ABSUser User { get; set; } = null!;
        public CreditPackage CreditPackage { get; set; } = null!;
    }
}
