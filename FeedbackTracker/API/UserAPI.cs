using System.Text.Json;
using Application.Components;
using Core.Definitions;
using Serilog;

namespace Application.API;

/// <summary>
/// User API client
/// </summary>
public class UserAPI
{
	private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(App.Endpoint) };

	/// <summary>
	/// Gets a user by their User ID.
	/// </summary>
	public async Task<User?> GetUserByID(int id)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync($"User/GetUserByID?ID={id}");
			response.EnsureSuccessStatusCode();
			string jsonString = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<User>(jsonString);
		}
		catch (Exception ex)
		{
			Log.Error(ex,$"Error getting user by ID {id}");
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
			var response = await _httpClient.GetAsync($"User/GetUserByUsername?Username={Uri.EscapeDataString(username)}");
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
	public async Task CreateUser(string username, string password, string email)
	{
		try
		{
			string url = $"User/CreateUser?Username={Uri.EscapeDataString(username)}" +
						 $"&Password={Uri.EscapeDataString(password)}" +
						 $"&Email={Uri.EscapeDataString(email)}";
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
	/// <param name="username">Username</param>
	/// <param name="password">Password</param>
	/// <param name="code">MFA Code (Optional)</param>
	/// <returns>JWT if authentication is successful, otherwise, "".</returns>
	public async Task<string> Authenticate(string username, string password, string code)
	{
		try
		{
			// Send login request.
			HttpResponseMessage response = await _httpClient.GetAsync(
				$"User/Authenticate?Username={Uri.EscapeDataString(username)}" +
				$"&Password={Uri.EscapeDataString(password)}&Code={code}");

			response.EnsureSuccessStatusCode();
			// Read the JWT token from the response
			return await response.Content.ReadAsStringAsync();

		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Authenticating user {username}");
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
            var response = await _httpClient.GetAsync(
	            $"User/GetModules?Userid={Uri.EscapeDataString(user.ToString())}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Modules?>?>(jsonString);
        }
        catch (Exception ex)
        {
	        Log.Error(ex, "Failed to get modules");
            return null;
        }

    }
    
    /// <summary>
    /// Enables 2FA for account
    /// </summary>
    /// <param name="userID">Account ID to enable 2FA for</param>
    /// <param name="password">Password for verification</param>
    /// <returns></returns>
    public async Task<bool> Enable2FA(string userID, string password)
    {
	    try
	    {
		    var endpoint = $"User/CreateTOTPKey?UserID={Uri.EscapeDataString(userID)}&Password={Uri.EscapeDataString(password)}";
		    var response = await _httpClient.GetAsync(endpoint);
		    response.EnsureSuccessStatusCode();
		    return true;
	    }
	    catch (Exception ex)
	    {
		    Log.Error(ex, $"Failed to enable 2FA for user {userID}");
		    return false;
	    }
    }

    public async Task<bool>GetMFAStatus(int userID)
    {
	    try 
	    {
		    var endpoint = $"User/GetMfaStatus?UserID={userID}";
		    var response = await _httpClient.GetAsync(endpoint);
		    response.EnsureSuccessStatusCode();
		    return bool.Parse(await response.Content.ReadAsStringAsync());
	    }
	    catch (Exception ex)
	    {
		    Log.Error(ex,"failed to get MFA Status");
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
	public async Task NewNotification(int userID, int feedbackID)
	{
		try
		{
			string url = $"User/Notification?Userid={Uri.EscapeDataString(userID.ToString())}" +
						 $"&FeedbackID={Uri.EscapeDataString(feedbackID.ToString())}";
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
    public async Task DeleteNotification(int userID)
    {
        try
        {
            string url = $"User/Notification?Userid={Uri.EscapeDataString(userID.ToString())}";
            using var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error Deleting notification");
        }
    }
	            
	/// <summary>
	/// Updates user password
	/// </summary>
	/// <param name="email">Account email</param>
	/// <param name="password">Account password</param>
	public async Task UpdatePassword(string email, string password)
	{
		try
		{
			string url = $"User/UpdatePassword?Email={Uri.EscapeDataString(email)}" +
			 $"&Password={Uri.EscapeDataString(password)}";
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