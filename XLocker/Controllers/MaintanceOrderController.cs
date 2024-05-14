using XLocker.DTOs.MaintanceOrder;
using XLocker.Entities;
using XLocker.Response.Common;
using XLocker.Response.Maintance;
using XLocker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintanceOrderController : ControllerBase
    {
        private readonly ILogger<MaintanceOrderController> _logger;
        private readonly IMaintanceService _maintanceService;
        private readonly UserManager<User> _userManager;

        public MaintanceOrderController(ILogger<MaintanceOrderController> logger, IMaintanceService maintanceService, UserManager<User> userManager)
        {
            _logger = logger;
            _maintanceService = maintanceService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "GetMaintanceOrder")]
        public async Task<ActionResult<ResponseList<MaintanceResponse>>> Get([FromQuery] GetMaintanceDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _maintanceService.Get(request));
            }
            return Ok(await _maintanceService.GetAll());
        }

        [HttpGet("Incoming")]
        [Authorize(Policy = "AssignMaintanceOrder")]
        public async Task<ActionResult<ResponseList<MaintanceResponse>>> Incoming([FromQuery] GetMaintanceDTO request)
        {
            return Ok(await _maintanceService.GetByStatus(request, Types.MaintanceStatus.MC));
        }

        [HttpGet("Overview")]
        [Authorize(Policy = "CompleteMaintanceOrder")]
        public async Task<ActionResult<ResponseList<MaintanceResponse>>> Overview([FromQuery] GetMaintanceDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            return Ok(await _maintanceService.GetAssinedToUser(request, user.Id));
        }

        [HttpPost]
        [Authorize(Policy = "CreateMaintanceOrder")]
        public async Task<ActionResult<ABSMaintanceOrder>> Create(CreateMaintanceDTO request)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            var maintanceOrder = await _maintanceService.Create(request, user.Id);
            return Ok(maintanceOrder);
        }

        [HttpPut("{maintanceId}/Assign")]
        [Authorize(Policy = "AssignMaintanceOrder")]
        public async Task<ActionResult<ABSMaintanceOrder>> Assign([FromRoute] string maintanceId, [FromBody] AssignMaintanceDTO request)
        {
            var maintanceOrder = await _maintanceService.Assign(request, maintanceId);
            return Ok(maintanceOrder);
        }

        [HttpPut("{maintanceId}/Complete")]
        [Authorize(Policy = "CompleteMaintanceOrder")]
        public async Task<ActionResult<ABSMaintanceOrder>> Complete([FromRoute] string maintanceId)
        {
            var maintanceOrder = await _maintanceService.Complete(maintanceId);
            return Ok(maintanceOrder);
        }

        [HttpPut("{maintanceId}")]
        [Authorize(Policy = "UpdateMaintanceOrder")]
        public async Task<ActionResult<bool>> Update([FromRoute] string maintanceId, [FromBody] UpdateMaintanceDTO request)
        {
            return Ok(await _maintanceService.Update(request, maintanceId));
        }

        [HttpDelete("{maintanceId}")]
        [Authorize(Policy = "DeleteMaintanceOrder")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string maintanceId)
        {
            return Ok(await _maintanceService.Delete(maintanceId));
        }
    }

}
