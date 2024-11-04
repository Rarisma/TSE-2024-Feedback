using Microsoft.AspNetCore.Mvc;

namespace Server.API;

public class Feedback : Controller 
{
	/// <summary>
	/// Get all feedbacks for the user
	/// </summary>
	/// <param name="UserID">User</param>
	/// <returns>List of Feedback Objects</returns>
	public string GetAssignedFeedbacks(int UserID)
	{

	}

	/// <summary>
	/// Gets feedback by its ID
	/// </summary>
	/// <param name="FeedbackID">Feedback ID</param>
	/// <returns>Feedback Object</returns>
	public string GetFeedbackByID(int FeedbackID)
	{

	}

	/// <summary>
	/// Creates new feedback entry in database
	/// </summary>
	/// <param name="feedback">Feedback object</param>
	/// <returns></returns>
	public void CreateFeedback(Feedback feedback)
	{

	}

	/// <summary>
	/// Updates feedback object
	/// </summary>
	/// <param name="feedback"></param>
	/// <returns></returns>
	public void UpdateFeedback(Feedback feedback)
	{

	}

	/// <summary>
	/// Deletes a feedback from the database
	/// </summary>
	/// <param name="FeedbackID"></param>
	public void DeleteFeedback(int FeedbackID)
	{

	}

	/// <summary>
	/// Gets comments for a thread
	/// </summary>
	/// <param name="FeedbackID">Feedback ID</param>
	/// <returns>Gets comments</returns>
	public string GetComments(int FeedbackID)
	{

	}
}