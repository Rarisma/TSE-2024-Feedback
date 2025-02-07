using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class User
{
	/// <summary>
	/// Users account ID
	/// </summary>
	[Key]
	[Column("USER_ID")]
	public int UserID { get; set; }

	/// <summary>
	/// User's username
	/// </summary>
	[Column("USERNAME")]
	public string Username { get; set; }

	/// <summary>
	/// User's password
	/// </summary>
	[Column("PASSWORD")]
	public string Password { get; set; }

	/// <summary>
	/// Is the user a student
	/// </summary>
	[Column("ISSTUDENT")]
	public bool IsStudent { get; set; }

	/// <summary>
	/// Is the user a teacher
	/// </summary>
	[Column("ISTEACHER")]
	public bool IsTeacher { get; set; }

	[Column("Email")]
	public string Email { get; set; }
}