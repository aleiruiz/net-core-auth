namespace XLocker.Response.Payment
{
    public class PaymentExcel
    {
        public string Id { get; set; } = string.Empty;

        public string GuideNumber { get; set; } = string.Empty;

        public float Amount { get; set; }

        public float Credits { get; set; }

        public string? Concept { get; set; }

        public string? UserName { get; set; }

        public string? CreditPackageName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
