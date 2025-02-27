using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

public class FeedbackFolders(int folderID, string userID, string feedbackID, User user, Feedback feedback)
{
	[Key]
	[Column("FOLDER_ID")]
	public int FolderID { get; set; } = folderID;

	[Column("USER_ID")]
	public string UserID { get; set; } = userID;

	[Column("FEEDBACK_ID")]
	public string FeedbackID { get; set; } = feedbackID;

	public User User { get; set; } = user;

	public Feedback Feedback { get; set; } = feedback;
}
