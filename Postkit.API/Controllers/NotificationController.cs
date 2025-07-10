using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Notifications.DTOs;
using Postkit.Notifications.Interfaces;
using Postkit.Shared.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;
        private readonly ILogger<NotificationController> logger;

        public NotificationController(INotificationService notificationService,
        ILogger<NotificationController> logger)
        {
            this.notificationService = notificationService;
            this.logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetAll()
        {
            logger.LogInformation("GET api/notifications called");

            var notifications = await notificationService.GetAllAsync();
            return Ok(ApiResponse<List<NotificationDto>>.SuccessResponse(notifications));
        }

        [HttpGet("unread")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetUnread()
        {
            logger.LogInformation("GET api/notifications/unread called");

            var unread = await notificationService.GetUnreadAsync();
            return Ok(ApiResponse<List<NotificationDto>>.SuccessResponse(unread));
        }

        [HttpPost("{id}/mark-as-read")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> MarkAsRead(Guid id)
        {
            logger.LogInformation("GET api/notifications/mark-as-read called");
            await notificationService.MarkAsReadAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Notification marked as read."));
        }

        [HttpPost("mark-all-as-read")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> MarkAllAsRead()
        {
            logger.LogInformation("GET api/notifications/mark-all-as-read called");
            await notificationService.MarkAllAsReadAsync();
            return Ok(ApiResponse<string>.SuccessResponse("All notifications marked as read."));
        }
    }
}
