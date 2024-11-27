using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

public class Modules
{
    [Key]
    public int ModuleID { get; set; }
    public string Module { get; set; }

}
