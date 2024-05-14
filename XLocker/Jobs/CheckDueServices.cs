using Newtonsoft.Json;
using XLocker.Services;

namespace XLocker.Jobs
{
    public class CheckDueServices
    {
        private IGuideService _guideService { get; }
        private IEmailService _emailService { get; }
        public CheckDueServices(IGuideService guideService, IEmailService emailService)
        {
            _guideService = guideService;
            _emailService = emailService;
        }

        public async Task<string> Execute()
        {
            var dueServices = await _guideService.GetDueServices();

            var updates = new List<string>();

            foreach (var service in dueServices)
            {
                try
                {
                    var result = await _guideService.ServiceBlocked(service.Id);
                    if (result != null)
                    {
                        updates.Add($"El servicio con el ID {service.Id} ha vencido.");
                        _ = await _emailService.DueService(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"El servicio con el ID {service.Id} no ha podido ser finalizado por el siguiente error.");
                    Console.WriteLine(ex.ToString());
                }
            }

            return JsonConvert.SerializeObject(updates);
        }
    }
}
