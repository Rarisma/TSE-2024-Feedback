using System.Text;
using System.Text.Json;
using Application.Components;
using Core.Definitions;
using Serilog;

namespace Application.API;

public class FeedbackAPI
{
	private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(App.Endpoint) };

	/// <summary>
	/// Get all feedbacks for the user.
	/// </summary>
	public async Task<List<Feedback>?> GetAssignedFeedbacks(int userID)
	{
		try
		{
			//Hit Endpoint
			HttpResponseMessage response = await _httpClient.GetAsync(
				$"Feedback/GetAssignedFeedbacks?userID={userID}");
			response.EnsureSuccessStatusCode();

			//Deserialize
			string jsonString = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<Feedback>>(jsonString);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Exception Getting assigned feedbacks");
			return null;
		}
	}

	/// <summary>
	/// Gets feedback by its ID.
	/// </summary>
	public async Task<Feedback?> GetFeedbackByID(int feedbackID)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync($"Feedback/GetFeedbackByID?feedbackID={feedbackID}");
			response.EnsureSuccessStatusCode();
			string jsonString = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<Feedback>(jsonString);
		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Exception getting feedback ID {feedbackID}");
			return null;
		}
	}

	/// <summary>
	/// Creates new feedback entry in the database.
	/// </summary>
	public async Task<string> CreateFeedback(Feedback feedback)
	{
		try
		{
			//Serialise
			string jsonContent = JsonSerializer.Serialize(feedback);
			StringContent content = new(jsonContent, Encoding.UTF8, "application/json");

			//Send
			HttpResponseMessage response = await _httpClient.PostAsync("Feedback/CreateFeedback", content);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Exception creating feedback");
			return $"Encountered an error: {ex.Message}";
		}
	}

	/// <summary>
	/// Deletes a feedback from the database.
	/// </summary>
	public async Task<string> DeleteFeedback(int feedbackID)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.DeleteAsync(
				$"Feedback/DeleteFeedback?feedbackID={feedbackID}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Exception deleting feedback id={feedbackID}");
			return $"Encountered an error: {ex.Message}";
		}
	}


	/// <summary>
	/// Creates a comment for a user
	/// </summary>
	/// <param name="feedbackID">ID of feedback comment is for</param>
	/// <param name="userID">UserID of Commenter</param>
	/// <param name="text">Comment body</param>
	/// <returns></returns>
    public async Task<string> CreateComment(int feedbackID, int userID, string? text)
    {
        try
        {
	        //Format for server
            StringContent commentText = new($"\"{text}\"", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(
	            $"Feedback/CreateComment?feedbackID={feedbackID}" +
	            $"&userID={userID}", commentText);
            
            //Send
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception creating comment");
            return $"Encountered an error: {ex.Message}";
        }
    }
	
    /// <summary>
    /// Gets comments for a thread.
    /// </summary>
    public async Task<List<FeedbackComments>?> GetComments(int feedbackID)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync(
				$"Feedback/GetComments?feedbackID={feedbackID}");
			response.EnsureSuccessStatusCode();
			string jsonString = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<List<FeedbackComments>>(jsonString);
		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Exception getting comments for feedback id={feedbackID}");
			return null;
		}
	}

    /// <summary>
    /// Updates request for a feedback
    /// </summary>
    public async Task UpdateFeedback(Feedback? feedback)
    {
        try
        {
            //Serialise
            string jsonContent = JsonSerializer.Serialize(feedback);
            StringContent content = new(jsonContent, Encoding.UTF8, "application/json");
            
            //Send
            HttpResponseMessage response = await _httpClient.PutAsync("Feedback/Feedback",
	            content);
            response.EnsureSuccessStatusCode();
			await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
	        Log.Error(ex, $"Exception updating feedback");
        }
    }
    
    /// <summary>
    /// Sets the state for the feedback
    /// </summary>
    /// <param name="FeedbackID">ID of feedback</param>
    /// <param name="State">True to close, false to open.</param>
    public async void SetStatus(int FeedbackID, bool State)
    {
	    HttpResponseMessage response = await _httpClient.GetAsync("Feedback/SetStatus?" +
	                                                              $"id={FeedbackID}&isOpen={State}");
	    response.EnsureSuccessStatusCode();
    }
}