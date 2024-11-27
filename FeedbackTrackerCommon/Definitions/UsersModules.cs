using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

public class UsersModules
{
    [Key]
    public int UserID { get; set; }
    public int ModuleID { get; set; }

    public User User { get; set; }
    public Modules Modules { get; set; }
}