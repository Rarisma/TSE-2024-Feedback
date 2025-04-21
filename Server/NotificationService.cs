using Core.Definitions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace Server
{
    /// <summary>
    /// Interface defining operations for managing user notifications.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Retrieves all notifications for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose notifications are to be retrieved.</param>
        /// <returns>A JSON string representing the list of notifications, or an error message.</returns>
        Task<string> GetNotificationsByUserAsync(int userId);

        /// <summary>
        /// Creates a new notification for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user for whom the notification is to be created.</param>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message content of the notification.</param>
        /// <param name="type">The type/category of the notification.</param>
        /// <param name="payload">Optional payload data associated with the notification.</param>
        /// <returns>True if the notification was created successfully; otherwise, false.</returns>
        Task<bool> CreateNotificationAsync(int userId, string title, string message, string type, string payload);

        /// <summary>
        /// Deletes all notifications for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose notifications should be deleted.</param>
        /// <returns>True if the notifications were deleted successfully; otherwise, false.</returns>
        Task<bool> DeleteNotificationsByUserAsync(int userId);

        /// <summary>
        /// Deletes notification by id.
        /// </summary>
        /// <param name="NotificationID">The ID of the notification.</param>
        /// <returns>True if the notifications were deleted successfully; otherwise, false.</returns>
        Task<bool> DeleteNotificationAsync(int NotificationID);
    }

    /// <summary>
    /// Concrete implementation of the INotificationService for managing user notifications.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly TrackerContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="context">The database context used for data access.</param>
        public NotificationService(TrackerContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<string> GetNotificationsByUserAsync(int userId)
        {
            try
            {
                var notifications = await _context.Notification
                    .Where(n => n.UserID == userId)
                    .Select(n => new
                    {
                        n.NotificationID,
                        n.UserID,
                        n.Title,
                        n.Message,
                        n.TimeSinceCreation,
                        n.Timestamp,
                        n.Type,
                        n.Payload
                    })
                    .ToListAsync();

                return JsonSerializer.Serialize(notifications);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving notifications");
                return JsonSerializer.Serialize(new { error = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CreateNotificationAsync(int userId, string title, string message, string type, string payload)
        {
            try
            {
                var notification = new Notification
                {
                    UserID = userId,
                    FeedbackId = 156, // TODO: Remove after merge
                    Title = title,
                    Message = message,
                    Timestamp = DateTime.Now,
                    Type = type,
                    Payload = payload
                };

                _context.Notification.Add(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create notification");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteNotificationsByUserAsync(int userId)
        {
            try
            {
                var notifications = await _context.Notification
                    .Where(n => n.UserID == userId)
                    .ToListAsync();

                _context.Notification.RemoveRange(notifications);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete notifications");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteNotificationAsync(int NotificationID)
        {
            try
            {
                var notifications = await _context.Notification
                    .Where(n => n.NotificationID == NotificationID)
                    .ToListAsync();

                _context.Notification.RemoveRange(notifications);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete notifications");
                return false;
            }
        }
    }
}
