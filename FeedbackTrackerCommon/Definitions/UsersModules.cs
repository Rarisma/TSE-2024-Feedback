using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class Users_Modules
{
	[Key]
	[Column("USER_ID")]
	public int UserID { get; set; }

	[Column("MODULE_ID")]
	public int ModuleID { get; set; }

    public User User { get; set; }
    public Modules Modules { get; set; }
}
