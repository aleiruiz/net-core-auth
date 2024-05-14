namespace XLocker.DTOs.Wallet
{
    public class CreateAwardDTO
    {
        public required string UserId { get; set; }

        public required int Credits { get; set; }

        public required string Concept { get; set; }
    }
}
