using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

    public class Notification
    {
        ///<summary>
        /// Notification
        /// </summary>
        [Key]
        [Column("NOTIFICATION_ID")]
        public int NotificationID { get; set; }

        ///<sumary>
        /// User
        /// </sumary>
        [Column("USER_ID")]
        public int? UserID { get; set; }

        ///<summary>
        /// Feedback
        /// </summary>
        [Column("FEEDBACK_ID")]
        public int FeedbackID { get; set; }

        ///<summary>
        /// TIME
        /// </summary>
        [Column("TIMESTAMP")]
        public DateTime Timestamp { get; set; }
    }

