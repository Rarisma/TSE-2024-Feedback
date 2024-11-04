using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class FolderLinks
{
    public int FolderID { get; set; }
    public int FeedbackID { get; set; }

    public FeedbackFolders FeedbackFolder { get; set; }
    public Feedback Feedback { get; set; }
}