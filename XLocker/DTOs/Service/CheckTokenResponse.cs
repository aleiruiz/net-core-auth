using XLocker.Response.Service;

namespace XLocker.DTOs.Service
{
    public class CheckTokenResponse
    {
        public VerboseServiceResponse Data { get; set; }
        public string? Message { get; set; }
        public bool CanOpen { get; set; }

        public float Total { get; set; }
    }
}
