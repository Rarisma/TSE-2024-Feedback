using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class CodeStorage
{
	///<summary>
	/// Extension ID
	/// </summary>
	[Key]
	[Column("CODE_ID")]
	public int CodeID { get; set; }

	[Column("USER_ID")]
	public int UserID { get; set; }

	[Column("CHECK_CODE")]
	public string CheckCode { get; set; }

	public User User { get; set; }
}
