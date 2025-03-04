using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

public class Users_Modules
{
	[Key]
	[Column("USER_ID")]
	public int UserID { get; init; }

	[Column("MODULE_ID")]
	public int ModuleID { get; init; }

    [ForeignKey("UserID")]
    public User User { get; init; }

    [ForeignKey("ModuleID")]
    public Modules Module { get; init; }
}
