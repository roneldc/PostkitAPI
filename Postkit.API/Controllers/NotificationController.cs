using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Notifications.DTOs;
using Postkit.Notifications.Interfaces;
using Postkit.Notifications.Queries;
using Postkit.Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Postkit.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/notifications")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SwaggerTag("Manages user notifications. Supports retrieval and marking notifications as read.")]
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

        /// <summary>
        /// Get all notifications for the current user.
        /// </summary>
        /// <remarks>Requires the user to be authenticated.</remarks>
        /// <response code="200">Returns a list of notifications</response>
        [Authorize]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all notifications", Description = "Retrieves all notifications for the currently authenticated user.")]
        [SwaggerResponse(200, "Notifications retrieved successfully", typeof(ApiResponse<List<NotificationDto>>))]
        public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetAll([FromQuery] NotificationQuery query)
        {
            logger.LogInformation("GET api/notifications called");
            query.ApiClientId = (Guid)HttpContext.Items["ApiClientId"]!;
            var notifications = await notificationService.GetAllAsync(query);
            return Ok(ApiResponse<PagedResponse<NotificationDto>>.SuccessResponse(notifications, "Notifications retrieved successfully"));
        }

        /// <summary>
        /// Mark a specific notification as read.
        /// </summary>
        /// <remarks>Requires the user to be authenticated and to own the notification.</remarks>
        /// <param name="id">The ID of the notification</param>
        /// <response code="200">Notification marked as read</response>
        /// <response code="404">Notification not found</response>
        [HttpPost("{id}/read")]
        [Authorize]
        [SwaggerOperation(Summary = "Mark notification as read", Description = "Marks a specific notification as read by ID.")]
        [SwaggerResponse(200, "Notification marked as read", typeof(ApiResponse<string>))]
        [SwaggerResponse(404, "Notification not found")]
        public async Task<ActionResult<ApiResponse<string>>> MarkAsRead(Guid id)
        {
            logger.LogInformation("GET api/notifications/read called");
            await notificationService.MarkAsReadAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Notification marked as read."));
        }

        /// <summary>
        /// Mark all notifications as read.
        /// </summary>
        /// <remarks>Marks all notifications of the authenticated user as read.</remarks>
        /// <response code="200">All notifications marked as read</response>
        [HttpPost("read-all")]
        [Authorize]
        [SwaggerOperation(Summary = "Mark all notifications as read", Description = "Marks all notifications of the current user as read.")]
        [SwaggerResponse(200, "All notifications marked as read", typeof(ApiResponse<string>))]
        public async Task<ActionResult<ApiResponse<string>>> MarkAllAsRead()
        {
            logger.LogInformation("GET api/notifications/read-all called");
            await notificationService.MarkAllAsReadAsync();
            return Ok(ApiResponse<string>.SuccessResponse("All notifications marked as read."));
        }
    }
}
