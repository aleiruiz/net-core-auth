using Newtonsoft.Json;
using XLocker.Services;

namespace XLocker.Jobs
{
    public class CheckPaymentStatus
    {
        private IPaymentService _paymentService { get; }
        public CheckPaymentStatus(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<string> Execute()
        {
            var pendingPayments = await _paymentService.GetPendingPayments();

            var updates = new List<string>();

            foreach (var payment in pendingPayments)
            {
                try
                {
                    var result = await _paymentService.CompletePurchase(payment.Id);
                    if (result != null)
                    {
                        updates.Add($"El pago con el ID {payment.Id} ha sido actualizado.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"El pago con el ID {payment.Id} no ha podido ser finalizado por el siguiente error.");
                    Console.WriteLine(ex.ToString());
                }
            }

            return JsonConvert.SerializeObject(updates);
        }
    }
}
