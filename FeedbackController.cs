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
		var Feedback = ctx.Feedback.First(f => f.FeedbackID == FeedbackID);
		ctx.Feedback.Remove(Feedback);
	}

    /// <summary>
    /// Create Commments
    /// </summary>
    [HttpPost("CreateComment")]
    public string CreateComment([FromBody] FeedbackComments commentsObject)
    {
        try
        {
            FeedbackComments? comments = commentsObject;


            //Add comment to database
            using TrackerContext Ctx = new();
            Ctx.FeedbackComments.Add(comments);
            Ctx.SaveChanges();

            return "Comment created successfully";
        }
        catch (Exception ex)
        {
            return "Encountered an error: " + ex.Message;
        }
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
	/// create extension
	/// </summary>
	/// <param name="ExtensionObject"></param>
	/// <returns></returns>
	[HttpPost("Extension")]
	public IActionResult ExtensionPost([FromBody] Extension? ExtensionObject)
	{
        try
        {
            Extension? extension = ExtensionObject;
            if (extension == null)
            {
	            return StatusCode(400);
            }

            //Add user to database
            using TrackerContext Ctx = new();
            Ctx.Extension.Add(extension);
            Ctx.SaveChanges();


            return StatusCode(200);
		}
        catch (Exception ex)
        {
			Log.Error(ex, "Failed to post extension correctly.");
	        return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets extension for feedback
    /// </summary>
    /// <param name="FeedbackID">Feedback ID</param>
    /// <returns>Gets comments</returns>
    [HttpGet("Extension")]
    public string ExtensionsGet(int FeedbackID)
    {
        try
        {
            using TrackerContext ctx = new();
			//Find all comments for the feedback
			List<Extension> extensions = ctx.Extension
                .Where(extension => extension.FeedbackId == FeedbackID).ToList();

            return JsonSerializer.Serialize(extensions);
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
    /// update extension
    /// </summary>
    /// <param name="extension">extension</param>
    /// <returns>Gets comments</returns>
    [HttpPut("Extension")]
    public IActionResult ExtensionPut([FromBody] Extension extension)
    {
        try
        {
			//Get feedback
            using TrackerContext ctx = new();
            var ext = ctx.Extension.SingleOrDefault(ex => ex.ExtensionId == extension.ExtensionId);

            if (ext != null)
            {
                // set feedback to new values
                ctx.Entry(ext).CurrentValues.SetValues(extension);
                //save, return success code.
                ctx.SaveChanges();
                return StatusCode(200);
			}

			//Extension invalid/not found, return 400
			return StatusCode(400);
        }
		catch (Exception ex)
        {
			Log.Error(ex, "Failed to put extension correctly");
	        return StatusCode(500);
        }
	}
}