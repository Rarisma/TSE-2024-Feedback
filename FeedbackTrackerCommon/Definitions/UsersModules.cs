using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

public class Users_Modules
{
	[Key]
	[Column("USER_ID")]
	public int UserID { get; set; }

	[Column("MODULE_ID")]
	public int ModuleID { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }

    [ForeignKey("ModuleID")]
    public Modules Module { get; set; }
}
