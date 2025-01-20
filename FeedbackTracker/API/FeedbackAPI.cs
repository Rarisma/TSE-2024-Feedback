using System.Text;
using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Serilog;
using Serilog.Core;

namespace Application.API;

public class FeedbackAPI(string baseEndpoint)
{
	private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(baseEndpoint) };

	/// <summary>
	/// Get all feedbacks for the user.
	/// </summary>
	public async Task<List<Feedback>?> GetAssignedFeedbacks(int? userID)
	{
		try
		{
			//Hit Endpoint
			HttpResponseMessage response = await _httpClient.GetAsync($"Feedback/GetAssignedFeedbacks?UserID={userID}");
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
			HttpResponseMessage response = await _httpClient.GetAsync($"Feedback/GetFeedbackByID?FeedbackID={feedbackID}");
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
			StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

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
			HttpResponseMessage response = await _httpClient.DeleteAsync($"Feedback/DeleteFeedback?FeedbackID={feedbackID}");
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
	/// Gets comments for a thread.
	/// </summary>
	public async Task<List<FeedbackComments>?> GetComments(int feedbackID)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync($"Feedback/GetComments?FeedbackID={feedbackID}");
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


	public async Task<List<User>?> GetAllUsersAsync()
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync("User/GetUsers");
			response.EnsureSuccessStatusCode();
			string jsonString = await response.Content.ReadAsStringAsync();
			var users = JsonSerializer.Deserialize<List<User>?>(jsonString);
			return await Task.FromResult(users);
		}
		catch (Exception ex)
		{
			return null;
		}
	}

	/// <summary>
	/// Creates extension request for a feedback
	/// </summary>
	/// <param name="extension"></param>
	/// <returns></returns>
	public async Task<string> CreateExtension(Extension extension)
	{
		try
		{
			//Serialise
			string jsonContent = JsonSerializer.Serialize(extension);
			StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

			//Send
			HttpResponseMessage response = await _httpClient.PostAsync("Feedback/Extension", content);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Exception creating extension");
			return $"Encountered an error: {ex.Message}";
		}
	}

    /// <summary>
    /// Gets extension request for a feedback
    /// </summary>
    public async Task<List<Extension>?> GetExtensions(int feedbackID)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Feedback/EXtension?FeedbackID={feedbackID}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Extension>>(jsonString);
        }
        catch (Exception ex)
        {
	        Log.Error(ex, $"Exception getting extension for feedback {feedbackID}");
			return null;
        }
    }

    /// <summary>
    /// Gets extension request for a feedback
    /// </summary>
    public async Task UpdateFeedback(Feedback feedback)
    {
        try
        {
            //Serialise
            string jsonContent = JsonSerializer.Serialize(feedback);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            //Send
            HttpResponseMessage response = await _httpClient.PutAsync("Feedback/Feedback", content);
            response.EnsureSuccessStatusCode();
			await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
	        Log.Error(ex, $"Exception updating feedback {feedback.FeedbackID}");
        }
    }

    /// <summary>
    /// Gets extension request for a feedback
    /// </summary>
    public async Task UpdateExtension(Extension extension)
    {
        try
        {
            //Serialise
            string jsonContent = JsonSerializer.Serialize(extension);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            //Send
            HttpResponseMessage response = await _httpClient.PutAsync("Feedback/Extension", content);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
	        Log.Error(ex, $"Exception updating extension {extension.ExtensionId}");
        }
    }

}