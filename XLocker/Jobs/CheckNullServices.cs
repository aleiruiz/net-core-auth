using Newtonsoft.Json;
using XLocker.Services;

namespace XLocker.Jobs
{
    public class CheckNullServices
    {
        private IGuideService _guideService { get; }
        private IEmailService _emailService { get; }
        public CheckNullServices(IGuideService guideService, IEmailService emailService)
        {
            _guideService = guideService;
            _emailService = emailService;
        }

        public async Task<string> Execute()
        {
            var serviceReminders = await _guideService.GetNullServices();

            var updates = new List<string>();

            foreach (var service in serviceReminders)
            {
                try
                {
                    var result = await _guideService.NullService(service.Id);
                    if (result != null)
                    {
                        updates.Add($"Se ha enviado un recordatorio a la guia con el Id {service.Id}.");
                        _ = await _emailService.Reminder(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"El servicio con el Id {service.Id} no ha podido ser contactado.");
                    Console.WriteLine(ex.ToString());
                }
            }

            return JsonConvert.SerializeObject(updates);
        }
    }
}
