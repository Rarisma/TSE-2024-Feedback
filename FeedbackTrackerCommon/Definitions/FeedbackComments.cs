using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

public class FeedbackComments
{
	/// <summary>
	/// ID of feedback comment in database.
	/// </summary>
	[Key]
	public int CommentID { get; set; }

	/// <summary>
	/// Content of comment
	/// </summary>
	public string Body { get; set; }

	/// <summary>
	/// Account ID of commenter
	/// </summary>
	public int CommenterID { get; set; }

    /// <summary>
    /// Feedback thread this comment belongs to
    /// </summary>
    public int FeedbackID { get; set; }

	public User Commenter { get; set; }
	public Feedback FeedbackC { get; set; }
}