namespace FeedbackTrackerCommon.Definitions;

public class FeedbackComments
{
	/// <summary>
	/// ID of feedback comment in database.
	/// </summary>
	public int CommentID;

	/// <summary>
	/// Feedback thread this comment belongs to
	/// </summary>
	public int FeedbackID;

	/// <summary>
	/// Content of comment
	/// </summary>
	public string Body;

	/// <summary>
	/// Account ID of commenter
	/// </summary>
	public int Commenter;
}