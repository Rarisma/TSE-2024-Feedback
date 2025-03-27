using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

    public class Notification
    {
        ///<summary>
        /// Notification
        /// </summary>
        [Key]
        [Column("NOTIFICATION_ID")]
        public int NotificationID { get; set; }

        ///<summary>
        /// User_ID
        /// </summary>
        [Column("USER_ID")]
        public int? UserID { get; set; }

        ///<summary>
        /// Title
        /// </summary>
        [Column("TITLE")]
        public string? Title { get; set; } = string.Empty;

        ///<summary>
        /// message
        /// </summary>
        [Column("MESSAGE")]
        public string? Message { get; set; } = string.Empty;

        ///<summary>
        /// TIME
        /// </summary>
        [Column("TIMESTAMP")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        ///<summary>
        /// TIME since creation
        /// </summary>
        public string TimeSinceCreation
        {
            get
            {
                var difference = DateTime.Now - Timestamp;

                if (difference.Days > 0)
                    return difference.Days.ToString() + "day(s) ago";

                if (difference.Hours > 0)
                    return difference.Hours.ToString() + "hour(s) ago";

                return difference.Minutes.ToString() + "min(s) ago";
            }
        }

    // remove when merged
    [Column("FEEDBACK_ID")]
    public int? FeedbackId { get; set; }
}

