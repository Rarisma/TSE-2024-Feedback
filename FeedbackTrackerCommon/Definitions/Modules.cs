using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

public class Modules
{
    [Key]
    private int ModuleID { get; set; }
    private int Module { get; set; }

}