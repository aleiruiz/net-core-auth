using XLocker.DTOs.Notification;
using XLocker.Entities;
using XLocker.Response.Common;
using XLocker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize(Policy = "GetNotification")]
        public async Task<ActionResult<ResponseList<Notification>>> Get([FromQuery] GetNotificationDTO request)
        {
            if (request.IsPaginated())
            {
                return Ok(await _notificationService.Get(request));
            }
            return Ok(await _notificationService.GetAll());
        }

        [HttpGet("User")]
        [Authorize]
        public async Task<ActionResult<ResponseList<Notification>>> GetUserNotification()
        {
            return Ok(await _notificationService.GetActiveNotifications());
        }


        [HttpPost]
        [Authorize(Policy = "CreateNotification")]
        public async Task<ActionResult<Notification>> Create(CreateNotificationDTO request)
        {
            var notification = await _notificationService.Create(request);
            return Ok(notification);
        }

        [HttpDelete("{notificationId}")]
        [Authorize(Policy = "DeleteNotification")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string notificationId)
        {
            return Ok(await _notificationService.Delete(notificationId));
        }
    }

}
