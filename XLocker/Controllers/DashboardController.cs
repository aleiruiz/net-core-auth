using Microsoft.AspNetCore.Mvc;
using XLocker.Response.Common;
using XLocker.Response.Dashboard;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : Controller
    {
        private readonly ILogger<DiagnosticController> _logger;
        private readonly IUserService _userService;
        private readonly ILockerService _lockerService;
        private readonly IMailboxService _mailboxService;
        private readonly IGuideService _guideService;

        public DashboardController(ILogger<DiagnosticController> logger, IUserService userService, ILockerService lockerService, IMailboxService mailboxService, IGuideService guideService)
        {
            _logger = logger;
            _userService = userService;
            _lockerService = lockerService;
            _mailboxService = mailboxService;
            _guideService = guideService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseList<DashboardResponse>>> Get()
        {
            var availableLockers = await _lockerService.GetActiveLockers();
            var availableMailbox = await _mailboxService.GetAvailableLockers();
            var serviceCompleted = await _guideService.GetCompletedServices();
            var serviceCreated = await _guideService.GetCreatedServices();
            var serviceOverdue = await _guideService.GetOverdueServices();
            var deliveryUsers = await _userService.GetDeliveryUsers();
            var activeService = await _guideService.GetActiveServices();

            return Ok(new DashboardResponse
            {
                LockersAvailable = availableLockers,
                MailboxesAvailable = availableMailbox,
                ServiceCompleted = serviceCompleted,
                ServiceCreated = serviceCreated,
                ServiceOverdue = serviceOverdue,
                DeliveryUsers = deliveryUsers,
                ActiveService = activeService,
            });
        }
    }
}
