using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.Wallet;
using XLocker.Entities;
using XLocker.Exceptions.User;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.Maintance;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ILogger<WalletController> _logger;
        private readonly IWalletService _walletService;
        private readonly UserManager<User> _userManager;
        private readonly IExcelService _excelService;
        private readonly IStorageService _storageService;

        public WalletController(ILogger<WalletController> logger, IWalletService walletService, UserManager<User> userManager, IExcelService excelService, IStorageService storageService)
        {
            _logger = logger;
            _walletService = walletService;
            _userManager = userManager;
            _excelService = excelService;
            _storageService = storageService;
        }

        [HttpGet("GetBalance")]
        [Authorize]
        public async Task<ActionResult<float>> GetBalance([FromQuery] GetWalletDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new UserDoesNotExistException();
            }
            return Ok(await _walletService.GetBalance(user.Id));
        }

        [HttpGet("GetTransactions")]
        [Authorize]
        public async Task<ActionResult<ResponseList<Wallet>>> GetTransactions([FromQuery] GetWalletDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new UserDoesNotExistException();
            }
            return Ok(await _walletService.GetTransactions(request, user.Id));
        }

        [HttpGet("GetTransactionExcel")]
        [Authorize]
        public async Task<ActionResult<ResponseList<Wallet>>> GetTransactionExcel([FromQuery] GetWalletDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new UserDoesNotExistException();
            }

            var excelList = await _walletService.GetBalanceReport(request, user.Id);
            var reportName = $"BalanceReport-{GenerateRandomNumber.GetRandomNumber(6)}.xls";
            var columnNames = new string[] { "Transaccion", "Cliente", "Creditos", "Concepto", "Tipo de Transaccion", "Fecha" };
            var excelMS = _excelService.CreateExcel(excelList, reportName, columnNames);

            var uri = await _storageService.UploadFileFromMemoryStream(excelMS, reportName);

            var res = new Response<string> { Data = uri, Status = 200 };

            return Ok(res);
        }

        [HttpPost("AwardCredits")]
        [Authorize(Policy = "AwardCredits")]
        public async Task<ActionResult<ABSDiagnostic>> AwardCredits(CreateAwardDTO request)
        {
            var wallet = await _walletService.Award(request);
            return Ok(wallet);
        }
    }

}
