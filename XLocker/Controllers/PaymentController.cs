using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.Package;
using XLocker.DTOs.Payment;
using XLocker.Entities;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.Maintance;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<User> _userManager;
        private readonly IExcelService _excelService;
        private readonly IStorageService _storageService;

        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService, UserManager<User> userManager, IExcelService excelService, IStorageService storageService)
        {
            _logger = logger;
            _paymentService = paymentService;
            _userManager = userManager;
            _excelService = excelService;
            _storageService = storageService;
        }

        [HttpGet]
        [Authorize(Policy = "GetAllPayments")]
        public async Task<ActionResult<ResponseList<DiagnosticResponse>>> GetAllPayments([FromQuery] GetPaymentDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _paymentService.Get(request));
            }
            return Ok(await _paymentService.GetAll());
        }



        [HttpGet("ExcelReport")]
        [Authorize(Policy = "GetAllPayments")]
        public async Task<ActionResult<Response<string>>> GetExcelReport([FromQuery] GetPaymentDTO request)
        {
            var excelList = await _paymentService.GetExcel(request);
            var reportName = $"PaymentReport-{GenerateRandomNumber.GetRandomNumber(6)}.xls";
            var columnNames = new string[] { "Id", "Numero de Guia BOLD", "Monto", "Creditos", "Concepto", "Cliente", "Paquete", "Fecha de Creacion", "Fecha de Pago" };
            var excelMS = _excelService.CreateExcel(excelList, reportName, columnNames);

            var uri = await _storageService.UploadFileFromMemoryStream(excelMS, reportName);

            var res = new Response<string> { Data = uri, Status = 200 };

            return Ok(res);
        }


        [HttpGet("GetMyPayments")]
        [Authorize]
        public async Task<ActionResult<ResponseList<DiagnosticResponse>>> GetMyPayments([FromQuery] GetPaymentDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            return Ok(await _paymentService.GetMyPayments(request, user.Id));
        }

        [HttpPost("PurchaseOrder")]
        [Authorize]
        public async Task<ActionResult<ABSDiagnostic>> CreatePurchaseOrder(PurchasePackageDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            var maintanceOrder = await _paymentService.CreatePurchaseOrder(request, user.Id);
            return Ok(maintanceOrder);
        }

        [HttpPost("CompletePurchase")]
        public async Task<ActionResult<ABSDiagnostic>> CompletePurchase(CompletePurchaseDTO request)
        {
            return Ok(await _paymentService.CompletePurchase(request.PurchaseId));
        }
    }

}
