using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.CreditPackage;
using XLocker.Entities;
using XLocker.Response.Common;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditPackageController : ControllerBase
    {
        private readonly ILogger<CreditPackageController> _logger;
        private readonly ICreditPackageService _creditPackageService;

        public CreditPackageController(ILogger<CreditPackageController> logger, ICreditPackageService creditPackageService)
        {
            _logger = logger;
            _creditPackageService = creditPackageService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseList<CreditPackage>>> Get([FromQuery] GetCreditPackageDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _creditPackageService.GetAll(request));
            }
            return Ok(await _creditPackageService.GetAll());
        }

        [HttpPost]
        [Authorize(Policy = "CreateCreditPackage")]
        public async Task<ActionResult<CreditPackage>> Create(CreateCreditPackageDTO request)
        {
            var maintanceOrder = await _creditPackageService.Create(request);
            return Ok(maintanceOrder);
        }

        [HttpPut("{creditPackageId}")]
        [Authorize(Policy = "UpdateCreditPackage")]
        public async Task<ActionResult<bool>> Update([FromRoute] string creditPackageId, [FromBody] UpdateCreditPackageDTO request)
        {
            return Ok(await _creditPackageService.Update(request, creditPackageId));
        }

        [HttpDelete("{creditPackageId}")]
        [Authorize(Policy = "DeleteCreditPackage")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string creditPackageId)
        {
            return Ok(await _creditPackageService.Delete(creditPackageId));
        }
    }

}
