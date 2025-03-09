using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

public class FeedbackComments
{
	/// <summary>
    /// ID of feedback comment in database.
    /// </summary>
    [Key]
	[Column("COMMENT_ID")]
	public int CommentID { get; set; }

    /// <summary>
    /// Content of comment
    /// </summary>
	[Column("BODY")]
	public string Body { get; set; }

	/// <summary>
    /// Account ID of commenter
    /// </summary>
	[Column("USER_ID")]

	public int CommenterID { get; set; }

	/// <summary>
    /// Feedback thread this comment belongs to
    /// </summary>
	[Column("FEEDBACK_ID")]
	public int FeedbackID { get; set; }
}
