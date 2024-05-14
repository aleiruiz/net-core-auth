using XLocker.Types;

namespace XLocker.Response.Wallet
{
    public class WalletExcel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserName { get; set; } = null!;

        public int Credits { get; set; }

        public string? Concept { get; set; }

        public TransactionType TransactionType { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}
