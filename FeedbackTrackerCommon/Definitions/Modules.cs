using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

public class Modules
{
    [Key]
	[Column("MODULE_ID")]
	public int ModuleID { get; set; }

	[Column("MODULE")]
	public string Module { get; set; }

}
