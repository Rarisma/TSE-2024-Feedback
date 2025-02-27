using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

public class Feedback(
    int feedbackID,
    int? assignedUserID,
    int assigneeID,
    int moduleID,
    string feedbackText,
    bool closed,
    string title,
    FeedbackLabel? label,
    FeedbackVisibility visibility,
    DateTime? createdDate,
    DateTime? closedDate,
    User assignedUser,
    User assignee,
    Modules module)
{
    /// <summary>
    /// ID of feedback in database.
    /// </summary>
    [Key]
    [Column("Feedback_ID")]
    public int FeedbackID { get; set; } = feedbackID;

    /// <summary>
    /// Person recieiving feedback
    /// </summary>
    /// 
    [Column("ASSIGNED_USER")]
    public int? AssignedUserID { get; set; } = assignedUserID;

    /// <summary>
    /// Person giving the feedback
    /// </summary>
    [Column("ASSIGNEE")]
    public int AssigneeID { get; set; } = assigneeID;

    /// <summary>
    /// Associated Module
    /// </summary>
    [Column("MODULE_ID")]
    public int ModuleID { get; set; } = moduleID;

    /// <summary>
    /// Body of feedback
    /// </summary>
    [Column("FEEDBACK_TEXT")]
    public string FeedbackText { get; set; } = feedbackText;

    /// <summary>
    /// Is the feedback open or closed
    /// </summary>
    [Column("COMPLETE")]
    public bool Closed { get; set; } = closed;

    /// <summary>
    /// Feedback title
    /// </summary>
    [Column("TITLE")]
    public string Title { get; set; } = title;

    [Column("Label")]
    public FeedbackLabel? Label { get; set; } = label;

    [Column("Visibility")]
    public FeedbackVisibility Visibility { get; set; } = visibility;

    [Column("CreatedDate")]
    public DateTime? CreatedDate { get; set; } = createdDate;

    [Column("ClosedDate")]
    public DateTime? ClosedDate { get; set; } = closedDate;

    public User AssignedUser { get; set; } = assignedUser;

    public User Assignee { get; set; } = assignee;

    public Modules Module { get; set; } = module;
}
