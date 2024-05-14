using XLocker.Services;

namespace XLocker.API
{
    public interface ISMSProvider
    {
        Task<bool> SendVerificationCode(string PhoneNumber, string Code);
    }

    public class AltiriaAPI(IHttpService httpService, IConfiguration configuration) : ISMSProvider
    {
        private readonly IHttpService _httpService = httpService;
        private readonly string endpoint = configuration["SMS:Endpoint"] ?? "";
        private readonly string apiKey = configuration["SMS:API_KEY"] ?? "";
        private readonly string apiSecret = configuration["SMS:API_SECRET"] ?? "";

        public async Task<bool> SendVerificationCode(string phoneNumber, string code)
        {
            string json = "{\"credentials\": {\"apiKey\":\"" + apiKey + "\",\"apiSecret\":\"" + apiSecret + "\"},";
            json += " \"destination\":[\"" + phoneNumber + "\"],";
            json += " \"message\": {\"msg\":\"" + $"Hola, su codigo de verificacion de XLocker es {code}" + "\"}}";
            var res = await _httpService.PostAsync(endpoint, json, "application/json");

            Console.Write(res);

            return true;
        }
    }

}
