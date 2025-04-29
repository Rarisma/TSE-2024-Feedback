using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

public class FeedbackFolders
{
	[Key]
	[Column("FOLDER_ID")]
	public int FolderID { get; set; }

	[Column("USER_ID")]
	public string UserID { get; set; }

	[Column("FEEDBACK_ID")]
	public string FeedbackID { get; set; }

	public User User { get; set; }

	public Feedback Feedback { get; set; }
}
