using System.Text.Json;
using System.Text.Json.Serialization;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Server.API;
[Route("Feedback")]
public class FeedbackController : Controller 
{
	/// <summary>
	/// Get all feedbacks for the user
	/// </summary>
	/// <param name="UserID">User</param>
	/// <returns>List of Feedback Objects</returns>
	[HttpGet("GetAssignedFeedbacks")]
	public string GetAssignedFeedbacks(int UserID)
	{
		try
		{

			//Find feedbacks for account
			using TrackerContext Ctx = new();
			List<Feedback> Feedback = Ctx.feedback
				.Where(f => f.AssignedUserID == UserID 
				|| f.AssignedUserID  == UserID).ToList();

			//Serialise to JSON
			return JsonSerializer.Serialize(Feedback);
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}

	/// <summary>
	/// Gets feedback by its ID
	/// </summary>
	/// <param name="FeedbackID">Feedback ID</param>
	/// <returns>Feedback Object</returns>
	[HttpGet("GetFeedbackByID")]	
	public string GetFeedbackByID(int FeedbackID)
	{
		try
		{
			//Find feedback
			using TrackerContext Ctx = new();
			Feedback Feedback = Ctx.feedback.First(f => f.FeedbackID == FeedbackID);

			//Serialise to JSON
			return JsonSerializer.Serialize(Feedback); ;
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}

	/// <summary>
	/// Creates new feedback entry in database
	/// </summary>
	/// <param name="FeedbackObject">Feedback object in json format</param>
	/// <returns></returns>
	[HttpPost("CreateFeedback")]
	public string CreateFeedback([FromBody]Feedback FeedbackObject)
	{
		try
		{
			//Deserialize
			/*
			Feedback? Feedback = JsonSerializer.Deserialize<Feedback>(FeedbackObject);
			*/
			Feedback? Feedback = FeedbackObject;
			if (Feedback == null)
			{
				return "Invalid feedback object";
			}
			
			Console.WriteLine($"feedback title: {Feedback.Title}"); // logging

			//Add user to database
			using TrackerContext Ctx = new();

            Console.WriteLine($"feedback text: {Feedback.FeedbackText}"); // logging

            Ctx.feedback.Add(Feedback);

            Console.WriteLine($"feedback priority: {Feedback.Priority}"); // logging

            Ctx.SaveChanges();

			Console.WriteLine("save");

			return "Feedback created successfully";
		}
		catch (Exception ex) {
			Console.WriteLine($"prev: {ex.InnerException.Message}  | error: {ex.Message}");
				return "Encountered an error: " + ex.Message; }
	}

	/*TODO:
	 Figure out what actually needs to be done for this
	/// <summary>
	/// Updates feedback object
	/// </summary>
	/// <param name="feedback"></param>
	/// <returns></returns>
	public void UpdateFeedback(FeedbackController feedback)
	{

	}*/

	/// <summary>
	/// Deletes a feedback from the database
	/// </summary>
	/// <param name="FeedbackID"></param>
	[HttpGet("DeleteFeedback")]	
	public void DeleteFeedback(int FeedbackID)
	{
		using TrackerContext ctx = new();

		//Delete feedback
		var Feedback = ctx.feedback.First(f => f.FeedbackID == FeedbackID);
		ctx.feedback.Remove(Feedback);
	}

	/// <summary>
	/// Gets comments for a thread
	/// </summary>
	/// <param name="FeedbackID">Feedback ID</param>
	/// <returns>Gets comments</returns>
	[HttpGet("GetComments")]	
	public string GetComments(int FeedbackID)
	{
		try
		{
			using TrackerContext ctx = new();
			//Find all comments for the feedback
			List<FeedbackComments> comments = ctx.feedback_comments
				.Where(comment => comment.FeedbackID == FeedbackID).ToList();
				
			return JsonSerializer.Serialize(comments);
		}
		catch (Exception e)
		{
			return "Encountered an error: " + e.Message;
		}
	}
}