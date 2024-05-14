using XLocker.Services;
using Newtonsoft.Json;

namespace XLocker.Jobs
{
    public class CheckOfflineLockers
    {
        private ILockerService _lockerService { get; }
        public CheckOfflineLockers(ILockerService lockerService)
        {
            _lockerService = lockerService;
        }

        public async Task<string> Execute()
        {
            var dueLOckers = await _lockerService.GetInactiveLockers();

            var updates = new List<string>();

            foreach (var locker in dueLOckers)
            {
                try
                {
                    var result = await _lockerService.OfflineLocker(locker.Id);
                    if (result)
                    {
                        updates.Add($"El locker  {locker.Name} esta desconectado.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"El locker {locker.Name} no se pudo actualizar");
                    Console.WriteLine(ex.ToString());
                }
            }

            return JsonConvert.SerializeObject(updates);
        }
    }
}
