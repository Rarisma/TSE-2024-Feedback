using System.ComponentModel.DataAnnotations;

namespace FeedbackTrackerCommon.Definitions;

public class User
{
    /// <summary>
    /// Users account ID
    /// </summary>
    [Key]
    public int UserID { get; set; }

    /// <summary>
    /// User's username
    /// </summary>
    public string Username { get; set; }

    public string Password { get; set; }
    
    /// <summary>
    /// Is the user a student
    /// </summary>
    public bool IsStudent { get; set; }
	
    /// <summary>
    /// Is the student a teacher
    /// </summary>
    public bool IsTeacher { get; set; }
}
