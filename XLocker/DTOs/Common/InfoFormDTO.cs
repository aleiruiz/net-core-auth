namespace XLocker.DTOs.Common
{
    public class InfoFormDTO
    {
        public string Message { get; set; } = string.Empty;

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerName { get; set; }
    }
}
