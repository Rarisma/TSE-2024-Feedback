using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

public class FeedbackComments(string body, int commenterID, int feedbackID)
{
	/// <summary>
    /// ID of feedback comment in database.
    /// </summary>
    [Key]
	[Column("COMMENT_ID")]
	public int CommentID { get; init; }

    /// <summary>
    /// Content of comment
    /// </summary>
	[Column("BODY")]
	public string Body { get; init; } = body;

	/// <summary>
    /// Account ID of commenter
    /// </summary>
	[Column("USER_ID")]

	public int CommenterID { get; init; } = commenterID;

	/// <summary>
    /// Feedback thread this comment belongs to
    /// </summary>
	[Column("FEEDBACK_ID")]
	public int FeedbackID { get; set; } = feedbackID;
}
