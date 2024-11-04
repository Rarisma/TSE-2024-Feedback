using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

public class FeedbackFolders
{
    [Key]
    public int FolderID { get; set; }
    public string UserID { get; set; }
    public string FeedbackID { get; set; }

    public User User { get; set; }

    public Feedback Feedback { get; set; }


}