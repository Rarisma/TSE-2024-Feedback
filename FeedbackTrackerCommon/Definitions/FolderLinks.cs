using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

public class FolderLinks
{
	  [Key]
	  public int FolderID { get; set; }
    public int FeedbackID { get; set; }

    public FeedbackFolders FeedbackFolder { get; set; }
    public Feedback Feedback { get; set; }
}