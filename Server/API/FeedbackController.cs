using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Core.Definitions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Server.API;
[Route("Feedback")]
public class FeedbackController : Controller 
{
	/// <summary>
	/// Get all feedbacks for the user
	/// </summary>
	/// <param name="userID">User</param>
	/// <returns>List of Feedback Objects</returns>
	[HttpGet("GetAssignedFeedbacks")]
	public string GetAssignedFeedbacks(int userID)
	{
		try
		{
			//Find feedbacks for account
			using TrackerContext ctx = new();

			List<Users_Modules> usersModules = ctx.UsersModules
				.Where(f => f.UserID == userID).ToList();

			//todo: remove this if not needed in future.
			//List<int> moduleIDs = ctx.UsersModules
			//	.Where(um => um.UserID == userID)
			//	.Select(um => um.ModuleID).ToList();
            List<int> userIDs = usersModules.Select(um => um.ModuleID).ToList();
            List<Feedback> feedback = ctx.Feedback
                .Where(f => f.AssignedUserID == userID
                         || f.AssigneeID == userID
                         || f.Visibility == FeedbackVisibility.Public
                         || f.AssignedUserID == null && userIDs.Contains(userID)).ToList ();


            //Serialise to JSON
            return JsonSerializer.Serialize(feedback);
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}

    /// <summary>
    /// Get all public feedbacks
    /// </summary>
    /// <returns>List of Feedback Objects</returns>
    [HttpGet("GetPublicFeedbacks")]
    public string GetPublicFeedbacks()
    {
        try
        {
			// Find public feedbacks
            using TrackerContext ctx = new();
            List<Feedback> publicFeedbacks = ctx.Feedback
                .Where(f => f.Visibility == FeedbackVisibility.Public).ToList();

			// Serialise to JSON
            return JsonSerializer.Serialize(publicFeedbacks);
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }
    
    /// <summary>
    /// Gets feedback by its ID
    /// </summary>
    /// <param name="feedbackID">Feedback ID</param>
    /// <returns>Feedback Object</returns>
    [HttpGet("GetFeedbackByID")]	
	public string GetFeedbackByID(int feedbackID)
	{
		try
		{
			//Find feedback
			using TrackerContext ctx = new();
			Feedback feedback = ctx.Feedback.First(f => f.FeedbackID == feedbackID);

			//Serialise to JSON
			return JsonSerializer.Serialize(feedback);
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}

	/// <summary>
	/// Creates new feedback entry in database
	/// </summary>
	/// <param name="feedbackObject">Feedback object in json format</param>
	/// <returns></returns>
	[HttpPost("CreateFeedback")]
	public string CreateFeedback([FromBody]Feedback? feedbackObject)
	{
		try
		{
			Feedback? feedback = feedbackObject;
			if (feedback == null)
			{
				return "Invalid feedback object";
			}
			
			//Add user to database, and save.
			using TrackerContext ctx = new();
			feedback.CreatedDate = DateTime.Now;
            var fb = ctx.Feedback.Add(feedback);
            ctx.SaveChanges();

			// notify

			Notification notification = new Notification
			{
				// notification id
				UserID = fb.Entity.AssignedUserID,
				/*
				FeedbackID = fb.Entity.FeedbackID,
				*/
				Timestamp = DateTime.Now,
			};

			ctx.Notification.Add(notification);
			ctx.SaveChanges();

			return "Feedback created successfully";
		}
		catch (Exception ex) {
				return "Encountered an error: " + ex.Message; }
	}
	
	/// <summary>
	/// Deletes a feedback from the database
	/// </summary>
	/// <param name="feedbackID"></param>
	[HttpGet("DeleteFeedback")]	
	public void DeleteFeedback(int feedbackID)
	{
		using TrackerContext ctx = new();

		//Delete feedback
		var feedback = ctx.Feedback.First(f => f.FeedbackID == feedbackID);
		ctx.Feedback.Remove(feedback);
	}
	
    /// <summary>
    /// Creates a comment
 	/// <param name="feedbackID">ID of feedback comment is for</param>
 	/// <param name="userID">User ID of commenter</param>
 	/// <param name="text">Comment content</param>
    /// </summary>
    [HttpPost("CreateComment")]
    public string CreateComment(int feedbackID, int userID, [FromBody] string? text)
    {
        try
        {
	        if (text != null)
	        {
		        FeedbackComments comment = new();
		        comment.Body = text;
		        comment.CommenterID =  userID;
		        comment.FeedbackID = feedbackID;
		        comment.CommentTime = DateTime.Now;
		        
		        //Add comment to database
		        using TrackerContext ctx = new();
		        ctx.FeedbackComments.Add(comment);
		        ctx.SaveChanges();
	        }

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
    /// <param name="feedbackID">Feedback ID</param>
    /// <returns>Gets comments</returns>
    [HttpGet("GetComments")]	
	public string GetComments(int feedbackID)
	{
		try
		{
			using TrackerContext ctx = new();
			//Find all comments for the feedback
			List<FeedbackComments> comments = ctx.FeedbackComments
				.Where(comment => comment.FeedbackID == feedbackID).ToList();
				
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
    /// <param name="feedback">Feedback</param>
    /// <returns>Gets comments</returns>
    [HttpPut("Feedback")]
    public IActionResult FeedbackPut([FromBody]Feedback? feedback)
    {
        try
        {
			//Check for null/invalid object.
			if (feedback == null) { return StatusCode(400); }

			//get feedback
			using TrackerContext ctx = new();
			Feedback? fb = ctx.Feedback.SingleOrDefault(fb => fb.FeedbackID == feedback.FeedbackID);
			
			if (fb != null)
			{
				// set feedback to new values
                ctx.Entry(fb).CurrentValues.SetValues(feedback);
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
    /// <param name="id">ID of feedback state is updating</param>
    /// <param name="isOpen">Should feedback be reopened/closed?</param>
    /// <returns></returns>
    [HttpGet("SetStatus")]	
    public IActionResult SetStatus(int id, bool isOpen)
    {
	    // Find feedback
	    using TrackerContext ctx = new();
	    Feedback? fb = ctx.Feedback.SingleOrDefault(fb => fb.FeedbackID == id);
	    
	    //update it
	    if (fb != null)
	    {
		    fb.Closed = isOpen;
		    //Updates title for closed feedback
		    fb.ClosedDate = DateTime.Now;
		    ctx.Feedback.Update(fb);
	    }

	    ctx.SaveChanges();
	    return StatusCode(200);
    }
}