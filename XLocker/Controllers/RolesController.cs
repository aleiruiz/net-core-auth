using XLocker.DTOs.Roles;
using XLocker.Entities;
using XLocker.Response.Common;
using XLocker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly IRoleService _roleService;

        public RolesController(ILogger<RolesController> logger, IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Policy = "GetRoles")]
        public async Task<ActionResult<ResponseList<ABSRole>>> Get([FromQuery] GetRoleDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _roleService.Get(request));
            }
            return Ok(await _roleService.GetAll());
        }

        [HttpGet("GetRolesManagment")]
        [Authorize(Policy = "GetRolesManagment")]
        public async Task<ActionResult<ResponseList<ABSRole>>> GetRolesManagment([FromQuery] GetRoleDTO request)
        {
            return Ok(await _roleService.GetManagmentRoles());
        }


        [HttpPost]
        [Authorize(Policy = "CreateRole")]
        public async Task<ActionResult<Role>> Create(CreateRoleDTO request)
        {
            var role = await _roleService.Create(request);
            return Ok(role);
        }

        [HttpPut("{roleId}")]
        [Authorize(Policy = "UpdateRole")]
        public async Task<ActionResult<bool>> Update([FromRoute] string roleId, [FromBody] UpdateRoleDTO request)
        {
            return Ok(await _roleService.Update(request, roleId));
        }

        [HttpDelete("{roleId}")]
        [Authorize(Policy = "DeleteRole")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string roleId)
        {
            return Ok(await _roleService.Delete(roleId));
        }
    }

}
