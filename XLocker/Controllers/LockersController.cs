using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.Locker;
using XLocker.Response.Common;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockersController : ControllerBase
    {
        private readonly ILogger<LockersController> _logger;
        private readonly ILockerService _lockerService;

        public LockersController(ILogger<LockersController> logger, ILockerService lockerService)
        {
            _logger = logger;
            _lockerService = lockerService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseList<ABSLocker>>> Get([FromQuery] GetLockerDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _lockerService.Get(request));
            }
            return Ok(await _lockerService.GetAllActive());
        }

        [HttpGet("Assignable")]
        public async Task<ActionResult<ResponseList<ABSLocker>>> GetAllAssignableLockers()
        {
            return Ok(await _lockerService.GetAllAssignableLockers());
        }

        [HttpPost]
        [Authorize(Policy = "CreateLocker")]
        public async Task<ActionResult<ABSLocker>> Create(CreateLockerDTO request)
        {
            var locker = await _lockerService.Create(request);
            return Ok(locker);
        }

        [HttpPut("{lockerId}")]
        [Authorize(Policy = "UpdateLocker")]
        public async Task<ActionResult<bool>> Update([FromRoute] string lockerId, [FromBody] UpdateLockerDTO request)
        {
            return Ok(await _lockerService.Update(request, lockerId));
        }

        [HttpPut("{lockerId}/Status")]
        [Authorize(Policy = "UpdateLockerStatus")]
        public async Task<ActionResult<bool>> UpdateLockerStatus([FromRoute] string lockerId, [FromBody] UpdateLockerStatusDTO request)
        {
            return Ok(await _lockerService.UpdateStatus(request, lockerId));
        }

        [HttpDelete("{lockerId}")]
        [Authorize(Policy = "DeleteLocker")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string lockerId)
        {
            return Ok(await _lockerService.Delete(lockerId));
        }
    }

}
