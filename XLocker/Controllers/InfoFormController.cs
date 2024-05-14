using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.Common;
using XLocker.Entities;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoFormController : ControllerBase
    {
        private readonly ILogger<ServicesController> _logger;
        private readonly IGuideService _guideService;
        private readonly IEmailService _emailService;
        private readonly IExcelService _excelService;
        private readonly IStorageService _storageService;
        private readonly UserManager<User> _userManager;

        public InfoFormController(ILogger<ServicesController> logger, IGuideService guideService, IEmailService emailService, UserManager<User> userManager, IExcelService excelService, IStorageService storageService)
        {
            _logger = logger;
            _guideService = guideService;
            _emailService = emailService;
            _userManager = userManager;
            _excelService = excelService;
            _storageService = storageService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<bool>> Post(InfoFormDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }

            request.CustomerName ??= user.Name;
            request.CustomerPhone ??= user.PhoneNumber;
            request.CustomerEmail ??= user.Email;

            var response = await _emailService.SubmitInfoForm(request);

            return Ok(response);
        }
    }

}
