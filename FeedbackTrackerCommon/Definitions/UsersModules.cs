using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class UsersModules
{
    [Key]
    [Column("USER_ID")]
    public int UserID { get; set; }
    [Column("MODULEID")]
    public int ModuleID { get; set; }

    //public User User { get; set; }
    //public Modules Modules { get; set; }
}