using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.Business;
using XLocker.Entities;
using XLocker.Response.Common;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly ILogger<BusinessController> _logger;
        private readonly IBusinessService _businessService;

        public BusinessController(ILogger<BusinessController> logger, IBusinessService businessService)
        {
            _logger = logger;
            _businessService = businessService;
        }

        [HttpGet]
        [Authorize(Policy = "GetBusiness")]
        public async Task<ActionResult<ResponseList<Business>>> Get([FromQuery] GetBusinessDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _businessService.GetAll(request));
            }
            return Ok(await _businessService.GetAll());
        }

        [HttpPost]
        [Authorize(Policy = "CreateBusiness")]
        public async Task<ActionResult<Business>> Create(CreateBusinessDTO request)
        {
            var maintanceOrder = await _businessService.Create(request);
            return Ok(maintanceOrder);
        }

        [HttpPut("{businessId}")]
        [Authorize(Policy = "UpdateBusiness")]
        public async Task<ActionResult<bool>> Update([FromRoute] string businessId, [FromBody] UpdateBusinessDTO request)
        {
            return Ok(await _businessService.Update(request, businessId));
        }

        [HttpDelete("{businessId}")]
        [Authorize(Policy = "DeleteBusiness")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string businessId)
        {
            return Ok(await _businessService.Delete(businessId));
        }
    }

}
