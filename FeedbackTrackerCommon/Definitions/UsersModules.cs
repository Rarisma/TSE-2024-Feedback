using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class UsersModules
{
    public int UserID { get; set; }
    public int ModuleID { get; set; }

    public User User { get; set; }
    public Modules Modules { get; set; }
}