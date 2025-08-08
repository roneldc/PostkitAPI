using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Postkit.Notifications.DTOs;
using Postkit.Notifications.Interfaces;
using Postkit.Shared.Interfaces.Auth;
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
        private readonly ICurrentUserService currentUser;

        public NotificationController(INotificationService notificationService,
        ILogger<NotificationController> logger,
        ICurrentUserService currentUser)
        {
            this.notificationService = notificationService;
            this.logger = logger;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Get all notifications for the current user.
        /// </summary>
        /// <remarks>Requires the user to be authenticated.</remarks>
        /// <response code="200">Returns a list of notifications</response>
        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Get all notifications", Description = "Retrieves all notifications for the currently authenticated user.")]
        [SwaggerResponse(200, "Notifications retrieved successfully", typeof(ApiResponse<PagedResponse<NotificationDto>>))]
        public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool unreadOnly = false)
        {
            logger.LogInformation("GET api/notifications called with page {Page}, pageSize {PageSize}, unreadOnly {UnreadOnly}", page, pageSize, unreadOnly);
            var userId = currentUser.UserId!;
            var result = await notificationService.GetNotificationsAsync(userId, page, pageSize, unreadOnly);
            return Ok(ApiResponse<PagedResponse<NotificationDto>>.SuccessResponse("Successfully retrieved notifications.", result));
        }


        /// <summary>
        /// Get notification summary (counts + recent)
        /// </summary>
        [HttpGet("summary")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get notification summary",
            Description = "Returns total unread count and recent notifications for the current user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the notification summary", typeof(ApiResponse<NotificationSummaryDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        public async Task<IActionResult> GetNotificationSummary()
        {

            logger.LogInformation("GET api/notifications/summary called");
            var userId = currentUser.UserId!;
            var summary = await notificationService.GetNotificationSummaryAsync(userId);
            return Ok(ApiResponse<NotificationSummaryDto>.SuccessResponse("Successfully retrieved notification summary", summary));
        }

        /// <summary>
        /// Marks a specific notification as read by the current user.
        /// </summary>
        /// <param name="notificationId">The ID of the notification to mark as read.</param>
        /// <returns>No content if successful; NotFound if the notification does not exist or is not accessible.</returns>
        [HttpPost("{notificationId}/read")]
        [Authorize]
        [SwaggerOperation(
                       Summary = "Mark notification as read",
                       Description = "Marks a specific notification as read for the current user."
                   )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Notification marked as read successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Notification failed to mark as read")]
        public async Task<IActionResult> MarkAsRead(string notificationId)
        {
            logger.LogInformation("POST api/notifications/{notificationId}/read called", notificationId);
            var userId = currentUser.UserId!;
            await notificationService.MarkAsReadAsync(notificationId, userId);
            return NoContent();
        }

        /// <summary>
        /// Marks all unread notifications as read for the current user.
        /// </summary>
        /// <returns>No content if successful; appropriate error response if the operation fails.</returns>
        [HttpPost("mark-all-read")]
        [Authorize]
        [SwaggerOperation(
                       Summary = "Mark all notifications as read",
                       Description = "Marks all unread notifications as read for the current user."
                   )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "All notifications marked as read successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Failed to mark all notifications as read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            logger.LogInformation("POST api/notifications/mark-all-read called");
            var userId = currentUser.UserId!;
            await notificationService.MarkAllAsReadAsync(userId);
            return NoContent();
        }

        /// <summary>
        /// Deletes a notification by its ID for the current user.
        /// </summary>
        /// <param name="notificationId">The ID of the notification to delete.</param>
        /// <returns>No content if successful; 404 if not found or doesn't belong to the user.</returns>
        [HttpDelete("{notificationId}")]
        [Authorize]
        [SwaggerOperation(
                                  Summary = "Delete a notification",
                                  Description = "Deletes a specific notification by its ID for the current user."
                              )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Notification deleted successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Notification not found or does not belong to the user")]
        public async Task<IActionResult> DeleteNotification(string notificationId)
        {
            logger.LogInformation("DELETE api/notifications/{notificationId} called", notificationId);
            var userId = currentUser.UserId!;
            await notificationService.DeleteNotificationAsync(notificationId, userId);
            return NoContent();
        }
    }
}
