using System.Text.Json;
using Application.Components;
using FeedbackTrackerCommon.Definitions;
using Serilog;

namespace Application.API;



public class UserAPI()
{
	private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(App.Endpoint) };


	/// <summary>
	/// Gets a user by their User ID.
	/// </summary>
	public async Task<User?> GetUserByID(int ID)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync($"User/GetUserByID?ID={ID}");
			response.EnsureSuccessStatusCode();
			string jsonString = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<User>(jsonString);
		}
		catch (Exception ex)
		{
			Log.Error(ex,$"Error getting user by ID {ID}");
			return null;
		}
	}

	/// <summary>
	/// Gets a user by their username.
	/// </summary>
	public async Task<User?> GetUserByUsername(string username)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync($"User/GetUserByUsername?Username={Uri.EscapeDataString(username)}");
			response.EnsureSuccessStatusCode();
			string jsonString = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<User>(jsonString);
		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Error getting user by name {username}");
			return null;
		}
	}

	/// <summary>
	/// Creates a new user.
	/// </summary>
	public async Task CreateUser(string Username, string Password, string Email, string School)
	{
		try
		{
			string url = $"User/CreateUser?Username={Uri.EscapeDataString(Username)}" +
						 $"&Password={Uri.EscapeDataString(Password)}" +
                         $"&School={Uri.EscapeDataString(School)}" +
                         $"&Email={Uri.EscapeDataString(Email)}";
			HttpResponseMessage response = await _httpClient.PostAsync(url, null);
			response.EnsureSuccessStatusCode();
			await response.Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Error creating user");
		}
	}

	/// <summary>
	/// Authenticates a user.
	/// </summary>
	/// <param name="Username">Username</param>
	/// <param name="Password">Password</param>
	/// <returns>JWT if authentication is successful, otherwise, "".</returns>

	public async Task<string> Authenticate(string Username, string Password, string Code)
	{
		try
		{
			// Send login request.
			HttpResponseMessage response = await _httpClient.GetAsync(
				$"User/Authenticate?Username={Uri.EscapeDataString(Username)}&Password={Uri.EscapeDataString(Password)}&Code={Code}");

			response.EnsureSuccessStatusCode();
			// Read the JWT token from the response
			return await response.Content.ReadAsStringAsync();

		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Authenticating user {Username}");
			return string.Empty;
		}
	}

    /// <summary>
    /// return users modules
    /// </summary>
    public async Task<List<Modules?>?> GetModules(int user)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"User/GetModules?Userid={Uri.EscapeDataString(user.ToString())}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Modules?>?>(jsonString);
        }
        catch (Exception ex)
        {
            return null;
        }

    }
    public async Task<bool> Enable2FA(string UserID, string Password)
    {
	    try
	    {
		    var endpoint = $"User/CreateTOTPKey?UserID={Uri.EscapeDataString(UserID)}&Password={Uri.EscapeDataString(Password)}";
		    HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
		    response.EnsureSuccessStatusCode();
		    return true;
	    }
	    catch (Exception ex)
	    {
		    return false;
	    }
    }

    public async Task<bool>getMFAStatus(int UserID)
    {
	    try 
	    {
		    var endpoint = $"User/MFABool?UserID={UserID}";
		    HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
		    response.EnsureSuccessStatusCode();
		    return Boolean.Parse(await response.Content.ReadAsStringAsync());
	    }
	    catch (Exception ex)
	    {
		    return false;
	    }

    }

    /// <summary>
    /// Get notifications
    /// </summary>
    public async Task<List<Notification?>?> GetNotification(int user)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"User/Notification?Userid={Uri.EscapeDataString(user.ToString())}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Notification?>?>(jsonString);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error getting notification");
            return null;
        }
    }

	/// <summary>
	/// Add new notification
	/// </summary>
	public async Task NewNotification(int UserID, int FeedbackID)
	{
		try
		{
			string url = $"User/Notification?Userid={Uri.EscapeDataString(UserID.ToString())}" +
						 $"&FeedbackID={Uri.EscapeDataString(FeedbackID.ToString())}";
			HttpResponseMessage response = await _httpClient.PostAsync(url, null);
			response.EnsureSuccessStatusCode();
			await response.Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Error creating notification");
		}
	}

            /// <summary>
            /// Delete notification store
            /// </summary>
    public async Task DeleteNotification(int UserID)
    {
        try
        {
            string url = $"User/Notification?Userid={Uri.EscapeDataString(UserID.ToString())}";
            HttpResponseMessage response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error Deleting notification");
        }
    }

	public async Task UpdatePassword(string Email, string Password)
	{
		try
		{
			string url = $"User/UpdatePassword?Email={Uri.EscapeDataString(Email)}" +
			 $"&Password={Uri.EscapeDataString(Password)}";
			HttpResponseMessage response = await _httpClient.PutAsync(url, null);
			response.EnsureSuccessStatusCode();
			await response.Content.ReadAsStringAsync();

		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Error updating password");
		}
	}


}