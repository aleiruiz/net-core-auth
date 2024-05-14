using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {

        [HttpGet]
        [Authorize(Policy = "GetPermissions")]
        public ActionResult<List<string>> Get()
        {
            var permissions = Enum.GetNames(typeof(Types.Permissions)).ToList();
            return Ok(permissions);
        }
    }
}
