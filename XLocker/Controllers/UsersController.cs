using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XLocker.DTOs.Users;
using XLocker.Exceptions.User;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = "GetUsers")]
        public async Task<ActionResult<ResponseList<ABSUser>>> Get([FromQuery] GetUsersDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _userService.Get(request));
            }
            return Ok(await _userService.GetAll());
        }


        [HttpGet("GetUsersManagment")]
        [Authorize(Policy = "GetUsersManagment")]
        public async Task<ActionResult<ResponseList<ABSUser>>> GetUsersManagment([FromQuery] GetUsersDTO request)
        {
            return Ok(await _userService.Get(request, "delivery,maintance"));
        }

        [HttpGet("Restricted")]
        [Authorize]
        public async Task<ActionResult<ResponseList<ABSUser>>> GetResticted([FromQuery] GetRestrictedUserDTO request)
        {
            return Ok(await _userService.GetRestrictedUserList(request));
        }

        [HttpPost]
        [Authorize(Policy = "CreateUser")]
        public async Task<ActionResult<ABSUser>> Create(AddUsersDTO request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new PasswordMismatchException();
            }
            return Ok(await _userService.Create(request));
        }

        [HttpPost("CreateUserManagment")]
        [Authorize(Policy = "CreateUserManagment")]
        public async Task<ActionResult<ABSUser>> CreateUserManagment(AddUsersDTO request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new PasswordMismatchException();
            }
            if (!ManagmentHelper.ManagmentAllowedRoles.Any(x => request.roles.Select(x => x.ToLower()).Contains(x)))
            {
                return Unauthorized();
            }
            return Ok(await _userService.Create(request));
        }

        [HttpPut("{userId}")]
        [Authorize(Policy = "UpdateUser")]
        public async Task<ActionResult<bool>> Update([FromRoute] string userId, [FromBody] UpdateUsersDTO request)
        {
            return Ok(await _userService.Update(request, userId));
        }

        [HttpPut("{userId}/UpdateUserManagment")]
        [Authorize(Policy = "UpdateUserManagment")]
        public async Task<ActionResult<bool>> UpdateUserManagment([FromRoute] string userId, [FromBody] UpdateUsersDTO request)
        {
            if (!ManagmentHelper.ManagmentAllowedRoles.Any(x => request.roles.Select(x => x.ToLower()).Contains(x)))
            {
                return Unauthorized();
            }
            return Ok(await _userService.Update(request, userId));
        }

        [HttpPut("{userId}/UpdatePassword")]
        [Authorize(Policy = "UpdateUserPasswords")]
        public async Task<ActionResult<bool>> UpdatePassword([FromRoute] string userId, [FromBody] UpdatePasswordDTO request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new PasswordMismatchException();
            }
            return Ok(await _userService.UpdatePassword(request, userId));
        }

        [HttpDelete("{userId}")]
        [Authorize(Policy = "DeleteUser")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string userId)
        {
            return Ok(await _userService.Delete(userId));
        }
    }
}
