using System.Net;
using System.Text;
using XLocker.DTOs.API;

namespace XLocker.Services
{
    public interface IHttpService
    {
        Task<string> GetAsync(string uri, List<HttpHeader> headers);
        Task<string> PostAsync(string uri, string data, string contentType);
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _client;

        public HttpService()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            };

            _client = new HttpClient();
        }

        public async Task<string> GetAsync(string uri, List<HttpHeader> headers)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(uri),
            };
            headers.ForEach(x => request.Headers.Add(x.Name, x.Value));
            using HttpResponseMessage response = await _client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostAsync(string uri, string data, string contentType)
        {
            try
            {
                using HttpContent content = new StringContent(data, Encoding.UTF8, contentType);

                HttpRequestMessage requestMessage = new HttpRequestMessage()
                {
                    Content = content,
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(uri)
                };

                using HttpResponseMessage response = await _client.SendAsync(requestMessage);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                throw new Exception();
            }
        }
    }
}
