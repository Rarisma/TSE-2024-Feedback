using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class Feedback
{
    /// <summary>
    /// ID of feedback in database.
    /// </summary>
    [Key]
    [Column("Feedback_ID")]
    public int FeedbackID { get; set; }

    /// <summary>
    /// Person recieiving feedback
    /// </summary>
    /// 
    [ForeignKey("AssignedUser")]
    [Column("ASSIGNED_USER")]
    public int AssignedUserID { get; set; }

    /// <summary>
    /// Person giving the feedback
    /// </summary>
    [ForeignKey("Assignee")]
    [Column("ASSIGNEE")]
    public int AssigneeID { get; set; }

    /// <summary>
    /// Associated Module
    /// </summary>
    [ForeignKey("Feedback")]
    [Column("MODULE_ID")]
    public int ModuleID { get; set; }

    /// <summary>
    /// Body of feedback
    /// </summary>
    [Column("FEEDBACK_TEXT")]
    public string FeedbackText { get; set; }

    /// <summary>
    /// Is the feedback open or closed
    /// </summary>
    [Column("COMPLETE")]
    public bool Closed { get; set; }

    /// <summary>
    /// Feedback title
    /// </summary>
    [Column("TITLE")]
    public string Title { get; set; }

    /// <summary>
    /// Feedback deadline
    /// </summary>
    [Column("Deadline")]
    public DateTime Deadline { get; set; }

    /// <summary>
    /// Priority Levels
    /// </summary>
    [Column("PRIORITY_VALUE")]
    public int Priority { get; set; }


    public User AssignedUser { get; set; }

    public User Assignee { get; set; }

    public Modules Module { get; set; }
}
