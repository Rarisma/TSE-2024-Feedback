namespace FeedbackTrackerCommon.Definitions;

public class Feedback
{
	/// <summary>
	/// ID of feedback in database.
	/// </summary>
	public int FeedbackID;

	/// <summary>
	/// Person recieiving feedback
	/// </summary>
	public int AssignedUser;

	/// <summary>
	/// Person giving the feedback
	/// </summary>
	public int Assignee;
	
	/// <summary>
	/// Body of feedback
	/// </summary>
	public string FeedbackText;

	/// <summary>
	/// Is the feedback open or closed
	/// </summary>
	public bool Closed;
	
	/// <summary>
	/// Feedback title
	/// </summary>
	public string Title;

	/// <summary>
	/// Priority Levels
	/// </summary>
	public int Priority;
}