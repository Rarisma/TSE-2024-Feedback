namespace FeedbackTrackerCommon.Definitions;

public class User
{
	/// <summary>
	/// Users account ID
	/// </summary>
    private int UserID;

	/// <summary>
	/// User's username
	/// </summary>
    private string Username;
    
	/// <summary>
	/// Is the user a student
	/// </summary>
	public bool IsStudent;

	/// <summary>
	/// Is the student a teacher
	/// </summary>
    public bool IsTeacher;
}