using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

public class Feedback
{
	/// <summary>
	/// ID of feedback in database.
	/// </summary>
	[Key]
	public int FeedbackID { get; set; }

	/// <summary>
	/// Person recieiving feedback
	/// </summary>
	public int AssignedUserID { get; set; }

	/// <summary>
	/// Person giving the feedback
	/// </summary>
	public int AssigneeID { get; set; }

    /// <summary>
    /// Associated Module
    /// </summary>
    public int ModuleID { get; set; }
	
	/// <summary>
	/// Body of feedback
	/// </summary>
	public string FeedbackText { get; set; }

	/// <summary>
	/// Is the feedback open or closed
	/// </summary>
	public bool Closed { get; set; }
	
	/// <summary>
	/// Feedback title
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// Priority Levels
	/// </summary>
	public int Priority { get; set; }

	public User AssignedUser { get; set; }
	public User Assignee { get; set; }
	public Modules Module { get; set; }
}