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
			
			//Add user to database
			using TrackerContext Ctx = new();

            Ctx.feedback.Add(Feedback);

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


	/// <summary>
	/// create extension
	/// </summary>
	/// <param name="extensionobject"></param>
	/// <returns></returns>
	[HttpPost("Extension")]
	public string ExtensionPost([FromBody] Extension extensionobject)
	{
        try
        {
            //Deserialize
            /*
			Feedback? Feedback = JsonSerializer.Deserialize<Feedback>(FeedbackObject);
			*/
            Extension? extension = extensionobject;
            if (extension == null)
            {
                return "Invalid extension object";
            }

            //Add user to database
            using TrackerContext Ctx = new();

            Ctx.extension.Add(extension);
			
            Ctx.SaveChanges();


            return "extension created successfully";
        }
        catch (Exception ex)
        {
            return " : Encountered an error: " + ex.Message;
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
			List<Extension> extensions = ctx.extension
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
    public string FeedbackPut([FromBody]Feedback feedback)
    {
        try
        {
            using TrackerContext ctx = new();
			//get feedback
			var fb = ctx.feedback.Single(feedbackid => feedbackid.FeedbackID == feedback.FeedbackID);
			
			if (fb != null)
			{
				// set feedback to new values
                ctx.Entry(fb).CurrentValues.SetValues(feedback);
				//save
				ctx.SaveChanges();
            }
			else
			{
				return "no feedback found";
			}

			



            return "feedback updated success";
        }
        catch (Exception e)
        {
            return "Encountered an error: " + e.Message;
        }
    }

    /// <summary>
    /// update extension
    /// </summary>
    /// <param name="extension">extension</param>
    /// <returns>Gets comments</returns>
    [HttpPut("Extension")]
    public string ExtensionPut([FromBody] Extension extension)
    {
        try
        {
            using TrackerContext ctx = new();
            //get feedback
            var ext = ctx.extension.Single(ex => ex.ExtensionId == extension.ExtensionId);

            if (ext != null)
            {
                // set feedback to new values
                ctx.Entry(ext).CurrentValues.SetValues(extension);
                //save
                ctx.SaveChanges();
            }
            else
            {
                return "no extension found";
            }

            return "extension updated success";
        }
        catch (Exception e)
        {
            return "Encountered an error: " + e.Message;
        }
    }
}