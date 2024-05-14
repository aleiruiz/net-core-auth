using System.Text.Json;
using XLocker.DTOs.API;
using XLocker.Response.API;
using XLocker.Services;

namespace XLocker.API
{
    public interface IBoldAPI
    {
        Task<BoldStatus> GetPaymentStatus(string guide);
    }

    public class BoldAPI(IHttpService httpService, IConfiguration configuration) : IBoldAPI
    {
        private readonly IHttpService _httpService = httpService;
        private readonly string endpoint = configuration["BOLD:API_ENDPOINT"] ?? "";
        private readonly string accessKey = configuration["BOLD:ACCESS_KEY"] ?? "";

        public async Task<BoldStatus> GetPaymentStatus(string guide)
        {
            var res = await _httpService.GetAsync($"{endpoint}v2/payment-voucher/{guide}", new List<HttpHeader> { new HttpHeader { Name = "Authorization", Value = $"x-api-key {accessKey}" } });

            return JsonSerializer.Deserialize<BoldStatus>(res)!;
        }
    }

}
