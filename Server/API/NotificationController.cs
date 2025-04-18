// NotificationController.cs
using Core.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.API.Controllers
{
    /// <summary>
    /// API controller for managing user notifications.
    /// Provides endpoints to retrieve, create, and delete notifications for users.
    /// </summary>
    [ApiController]
    [Route("notification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="notificationService">Service for handling notification operations.</param>
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Retrieves all notifications associated with a specific user ID.
        /// </summary>
        /// <param name="userid">The ID of the user whose notifications should be retrieved.</param>
        /// <returns>Returns a 200 OK response with the list of notifications in JSON format.</returns>
        [HttpGet("ByUser")]
        public async Task<IActionResult> GetByUser(int userid)
        {
            var result = await _notificationService.GetNotificationsByUserAsync(userid);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new notification for a user.
        /// </summary>
        /// <param name="userid">The ID of the user to receive the notification.</param>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The body message of the notification.</param>
        /// <param name="type">The type or category of the notification.</param>
        /// <param name="payload">Optional payload data associated with the notification.</param>
        /// <returns>
        /// Returns a 200 OK response if successful; otherwise, a 500 Internal Server Error.
        /// </returns>
        [HttpPost("")]
        public async Task<IActionResult> CreateNotification(int userid, string title, string message, string type, string payload)
        {
            var result = await _notificationService.CreateNotificationAsync(userid, title, message, type, payload);
            return result ? Ok("Notification created successfully") : StatusCode(500, "Failed to create notification");
        }

        /// <summary>
        /// Deletes all notifications associated with a specific user ID.
        /// </summary>
        /// <param name="userID">The ID of the user whose notifications should be deleted.</param>
        /// <returns>
        /// Returns a 200 OK response if successful; otherwise, a 500 Internal Server Error.
        /// </returns>
        [HttpDelete("ByUser")]
        public async Task<IActionResult> DeleteByUser(int userID)
        {
            var result = await _notificationService.DeleteNotificationsByUserAsync(userID);
            return result ? Ok("Notifications deleted successfully") : StatusCode(500, "Failed to delete notifications");
        }
    }
}
