using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.Locker;
using XLocker.DTOs.Service;
using XLocker.Entities;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.Service;
using XLocker.Services;
using XLocker.Types;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ILogger<ServicesController> _logger;
        private readonly IGuideService _guideService;
        private readonly IEmailService _emailService;
        private readonly IExcelService _excelService;
        private readonly IStorageService _storageService;
        private readonly UserManager<User> _userManager;

        public ServicesController(ILogger<ServicesController> logger, IGuideService guideService, IEmailService emailService, UserManager<User> userManager, IExcelService excelService, IStorageService storageService)
        {
            _logger = logger;
            _guideService = guideService;
            _emailService = emailService;
            _userManager = userManager;
            _excelService = excelService;
            _storageService = storageService;
        }

        [HttpGet]
        [Authorize(Policy = "GetService")]
        public async Task<ActionResult<ResponseList<ServiceResponse>>> Get([FromQuery] GetServiceDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _guideService.Get(request));
            }
            return Ok(await _guideService.GetAll());
        }

        [HttpGet("ExcelReport")]
        [Authorize(Policy = "GetService")]
        public async Task<ActionResult<Response<string>>> GetExcelReport([FromQuery] GetServiceDTO request)
        {
            var excelList = await _guideService.GetExcel(request);
            var reportName = $"ServiceReport-{GenerateRandomNumber.GetRandomNumber(6)}.xls";
            var columnNames = new string[] { "Cliente", "Email", "Telefono", "Costo Final", "Casillero", "Taquilla", "Fecha de Deposito", "Fecha de Retiro", "Fecha de Novedad", "Estado", "Tipo de Novedad", "Fecha de Creacion", "Fecha de Actualizacion" };
            var excelMS = _excelService.CreateExcel(excelList, reportName, columnNames);

            var uri = await _storageService.UploadFileFromMemoryStream(excelMS, reportName);

            var res = new Response<string> { Data = uri, Status = 200 };

            return Ok(res);
        }


        [HttpGet("WithdrawalPending")]
        [Authorize(Policy = "CreateWithdrawalOrderService")]
        public async Task<ActionResult<ResponseList<ServiceResponse>>> WithdrawalPending([FromQuery] GetServiceDTO request)
        {
            return Ok(await _guideService.GetByStatus(request, [ServiceStatus.SV]));
        }

        [HttpGet("ActiveService")]
        [Authorize]
        public async Task<ActionResult<ActiveServiceResponse>> GetActiveService([FromQuery] GetLockerDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            var service = await _guideService.GetActiveService(user.Id);

            return Ok(
                new ActiveServiceResponse
                {
                    Service = service,
                });
        }


        [HttpGet("GetMyServices")]
        [Authorize]
        public async Task<ActionResult<ResponseList<ServiceResponse>>> GetMyServices([FromQuery] GetServiceDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            return Ok(await _guideService.GetMyServices(request, user.Id));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ABSService>> Create(CreateServiceDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            var service = await _guideService.Create(request, user.Id);
            var fullService = await _guideService.GetServiceById(service.Id);

            _ = await _emailService.Deposit(fullService);
            return Ok(service);
        }

        [HttpPut("{lockerId}/CheckToken")]
        public async Task<ActionResult<VerboseServiceResponse>> CheckCourierToken(CheckTokenDTO request, string lockerId)
        {
            var service = await _guideService.CheckToken(request, lockerId);
            return Ok(service);
        }

        [HttpPut("{serviceId}/DepositPackage")]
        public async Task<ActionResult<bool>> DepositPackage(DepositServiceDTO request, string serviceId)
        {
            var response = await _guideService.DepositPackage(request, serviceId);

            return Ok(response);
        }

        [HttpPut("{serviceId}/WithdrawPackage")]
        public async Task<ActionResult<bool>> WithdrawPackage(WithdrawServiceDTO request, string serviceId)
        {
            var response = await _guideService.WithdrawPackage(request, serviceId);

            var service = await _guideService.GetServiceById(serviceId);

            _ = await _emailService.Withdrawl(service);

            return Ok(response);
        }

        [HttpPut("{serviceId}/CreateNovelty")]
        [Authorize(Policy = "CreateNovelty")]
        public async Task<ActionResult<bool>> CreateNovelty(NoveltyServiceDTO request, string serviceId)
        {
            var service = await _guideService.CreateNovelty(request, serviceId);
            return Ok(service);
        }

        [HttpPut("{lockerId}/CheckSupportToken")]
        public async Task<ActionResult<ServiceResponse>> CheckSupportToken(CheckTokenDTO request, string lockerId)
        {
            var service = await _guideService.CheckSupportToken(request, lockerId);
            return Ok(service);
        }

        [HttpPut("{serviceId}/CancelService")]
        [Authorize(Policy = "CancelService")]
        public async Task<ActionResult<Service>> CancelService([FromRoute] string serviceId)
        {
            return Ok(await _guideService.CancelService(serviceId));
        }

        [HttpDelete("{serviceId}")]
        [Authorize(Policy = "DeleteService")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string serviceId)
        {
            return Ok(await _guideService.Delete(serviceId));
        }
    }

}
