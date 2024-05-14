using Newtonsoft.Json;
using XLocker.Services;

namespace XLocker.Jobs
{
    public class CheckPendingServices
    {
        private IGuideService _guideService { get; }
        public CheckPendingServices(IGuideService guideService)
        {
            _guideService = guideService;
        }

        public async Task<string> Execute()
        {
            var dueServices = await _guideService.GetPendingService();

            var updates = new List<string>();

            foreach (var service in dueServices)
            {
                try
                {
                    var result = await _guideService.CancelService(service.Id);
                    if (result)
                    {
                        updates.Add($"El servicio con el Id {service.Id} no ha sido utilizado a tiempo.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"El servicio con el Id {service.Id} no ha podido ser finalizado por el siguiente error.");
                    Console.WriteLine(ex.ToString());
                }
            }

            return JsonConvert.SerializeObject(updates);
        }
    }
}
