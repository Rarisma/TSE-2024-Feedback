using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
			List<Feedback> Feedback = Ctx.Feedback
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
			Feedback Feedback = Ctx.Feedback.First(f => f.FeedbackID == FeedbackID);

			//Serialise to JSON
			return JsonSerializer.Serialize(Feedback);
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
			
			//Add user to database, and save.
			using TrackerContext Ctx = new();
            var fb = Ctx.Feedback.Add(Feedback);
            Ctx.SaveChanges();

			// notify

			Notification notification = new Notification
			{
				// notification id
				UserID = fb.Entity.AssignedUserID,
				FeedbackID = fb.Entity.FeedbackID,
				Timestamp = DateTime.Now,
			};

			Ctx.Notification.Add(notification);
			Ctx.SaveChanges();

			return "Feedback created successfully";
		}
		catch (Exception ex) {
				return "Encountered an error: " + ex.Message; }
	}
	

	/// <summary>
	/// Deletes a feedback from the database
	/// </summary>
	/// <param name="FeedbackID"></param>
	[HttpGet("DeleteFeedback")]	
	public void DeleteFeedback(int FeedbackID)
	{
		using TrackerContext ctx = new();

		//Delete feedback
		var Feedback = ctx.Feedback.First(f => f.FeedbackID == FeedbackID);
		ctx.Feedback.Remove(Feedback);
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
			List<FeedbackComments> comments = ctx.FeedbackComments
				.Where(comment => comment.FeedbackID == FeedbackID).ToList();
				
			return JsonSerializer.Serialize(comments);
		}
		catch (Exception e)
		{
			return "Encountered an error: " + e.Message;
		}
	}


    /// <summary>
    /// update Feedback
    /// </summary>
    /// <param name="Feedback">Feedback</param>
    /// <returns>Gets comments</returns>
    [HttpPut("Feedback")]
    public IActionResult FeedbackPut([FromBody]Feedback? Feedback)
    {
        try
        {
			//Check for null/invalid object.
			if (Feedback == null) { return StatusCode(400); }

			//get feedback
			using TrackerContext ctx = new();
			Feedback? fb = ctx.Feedback.SingleOrDefault(fb => fb.FeedbackID == Feedback.FeedbackID);
			
			if (fb != null)
			{
				// set feedback to new values
                ctx.Entry(fb).CurrentValues.SetValues(Feedback);
				//save, return success
				ctx.SaveChanges();
				return StatusCode(200);
			}
			//Not found, return 404.
			return StatusCode(404);
		}
		catch (Exception e)
        {
			Log.Error(e, "failed to update the ");
	        return StatusCode(500);
        }
	}
    
    /// <summary>
    /// Sets the completion state of a feedback 
    /// </summary>
    /// <param name="IsOpen"></param>
    /// <returns></returns>
    [HttpGet("SetStatus")]	
    public IActionResult SetStatus(int ID, bool IsOpen)
    {
	    // Find feedback
	    using TrackerContext ctx = new();
	    Feedback? fb = ctx.Feedback.SingleOrDefault(fb => fb.FeedbackID == ID);
	    
	    //update it
	    fb.Closed = IsOpen;
	    ctx.Feedback.Update(fb);
	    ctx.SaveChanges();
	    return StatusCode(200);
    }
}