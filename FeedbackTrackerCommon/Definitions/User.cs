using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Definitions;

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
    /// users first name
    /// </summary>
    [Column("FIRST_NAME")]
    public string? FirstName { get; set; }

    /// <summary>
    /// users first name
    /// </summary>
    [Column("LAST_NAME")]
    public string? LastName { get; set; }

    /// <summary>
    /// User's password
    /// </summary>
    [Column("PASSWORD")]
	public string Password { get; set; }


	/// <summary>
	/// Is the user a teacher
	/// </summary>
	[Column("ISTEACHER")]
	public bool IsTeacher { get; set; }
  
	[Column("Email")]
	public string Email { get; set; }

	[Column("LASTLOGIN")]
	public DateTime? LastLogin { get; set; }

	[Column("TOTP")]
	public string? MFASecret { get; set; }

  [Column("School")]
  public string? School { get; set; }
	/// <summary>
	/// Shows a user a setup page when logging in for the first time if false
	/// (used in Bulk API)
	/// </summary>
	[Column("INITIALISED")]
	public bool Initalised { get; set; }
}