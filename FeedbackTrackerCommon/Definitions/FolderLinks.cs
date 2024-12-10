using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class FolderLinks
{
	[Key]
	[Column("FOLDER_ID")]
	public int FolderID { get; set; }

	[Column("FEEDBACK_ID")]
	public int FeedbackID { get; set; }

    public FeedbackFolders FeedbackFolder { get; set; }
    public Feedback Feedback { get; set; }
}
