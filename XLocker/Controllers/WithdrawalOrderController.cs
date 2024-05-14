using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.WithdrawalOrder;
using XLocker.Entities;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.Service;
using XLocker.Response.WithdrawalOrder;
using XLocker.Services;
using XLocker.Types;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawalOrderController : ControllerBase
    {
        private readonly ILogger<WithdrawalOrderController> _logger;
        private readonly IWithdrawalOrderService _withdrawalOrderService;
        private readonly IExcelService _excelService;
        private readonly IStorageService _storageService;
        private readonly UserManager<User> _userManager;

        public WithdrawalOrderController(ILogger<WithdrawalOrderController> logger, IWithdrawalOrderService withdrawalOrderService, UserManager<User> userManager, IExcelService excelService, IStorageService storageService)
        {
            _logger = logger;
            _withdrawalOrderService = withdrawalOrderService;
            _userManager = userManager;
            _excelService = excelService;
            _storageService = storageService;
        }

        [HttpGet]
        [Authorize(Policy = "GetWithdrawalOrder")]
        public async Task<ActionResult<ResponseList<WithdrawalOrderResponse>>> Get([FromQuery] GetWithdrawalOrderDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _withdrawalOrderService.Get(request));
            }
            return Ok(await _withdrawalOrderService.GetAll());
        }

        [HttpGet("ExcelReport")]
        [Authorize(Policy = "GetWithdrawalOrder")]
        public async Task<ActionResult<ResponseList<WithdrawalOrderResponse>>> GetExcelReport([FromQuery] GetWithdrawalOrderDTO request)
        {
            var excelList = await _withdrawalOrderService.GetExcel(request);
            var reportName = $"WithdrawalReport-{GenerateRandomNumber.GetRandomNumber(6)}.xls";
            var columnNames = new string[] { "Cliente", "Email", "Casillero", "Taquilla", "Estado", "Fecha de Retiro", "Fecha de Creacion", "Fecha de Actualizacion" };
            var excelMS = _excelService.CreateExcel(excelList, reportName, columnNames);

            var uri = await _storageService.UploadFileFromMemoryStream(excelMS, reportName);

            var res = new Response<string> { Data = uri, Status = 200 };

            return Ok(res);
        }

        [HttpGet("WithdrawalsCompleted")]
        public async Task<ActionResult<ResponseList<WithdrawalOrderResponse>>> GetDeliveredServices([FromQuery] GetWithdrawalOrderDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            return Ok(await _withdrawalOrderService.GetCompletedWithdrawals(request, user.Id));
        }

        [HttpPost]
        [Authorize(Policy = "CreateWithdrawalOrderService")]
        public async Task<ActionResult<ABSWithdrawalOrder>> Create(CreateWithdrawalOrderDTO request)
        {
            var service = await _withdrawalOrderService.Create(request);
            return Ok(service);
        }

        [HttpGet("Incoming")]
        [Authorize(Policy = "AssignWithdrawalOrderService")]
        public async Task<ActionResult<ResponseList<ServiceResponse>>> GetIncomingServices([FromQuery] GetWithdrawalOrderDTO request)
        {
            return Ok(await _withdrawalOrderService.GetByStatus(request, [ServiceStatus.SVC]));
        }

        [HttpGet("Overview")]
        [Authorize]
        public async Task<ActionResult<ResponseList<ServiceResponse>>> GetOverview([FromQuery] GetWithdrawalOrderDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            return Ok(await _withdrawalOrderService.GetAssignedToUser(request, user.Id));
        }

        [HttpPut("{orderId}/Assign")]
        [Authorize(Policy = "AssignWithdrawalOrderService")]
        public async Task<ActionResult<bool>> AssignOrder(AssignWithdrawalOrderDTO request, string orderId)
        {
            var service = await _withdrawalOrderService.Assign(request, orderId);
            return Ok(service);
        }

        [HttpPut("{orderId}/Complete")]
        public async Task<ActionResult<bool>> CompleteOrder(CompleteWithdrawalOrderDTO request, string orderId)
        {
            var response = await _withdrawalOrderService.Complete(request, orderId);


            return Ok(response);
        }
    }

}
