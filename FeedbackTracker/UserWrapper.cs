using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Application;

public class UserAPI
{
	private readonly HttpClient _httpClient;

	public UserAPI(string baseEndpoint)
	{
		_httpClient = new HttpClient { BaseAddress = new Uri(baseEndpoint) };
	}

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
			return null;
		}
	}

	/// <summary>
	/// Creates a new user.
	/// </summary>
	public async Task<string> CreateUser(string Username, string Password)
	{
		try
		{
			string url = $"User/CreateUser?Username={Uri.EscapeDataString(Username)}" +
						 $"&Password={Uri.EscapeDataString(Password)}";
			HttpResponseMessage response = await _httpClient.PostAsync(url, null);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			return $"Error: {ex.Message}";
		}
	}

	/// <summary>
	/// Authenticates a user.
	/// </summary>
	/// <param name="Username">Username</param>
	/// <param name="Password">Password</param>
	/// <returns>JWT if authentication is successful, otherwise, "".</returns>
	public async Task<string> Authenticate(string Username, string Password)
	{
		try
		{
			// Send login request.
			HttpResponseMessage response = await _httpClient.GetAsync($"User/Authenticate?Username={Username}&Password={Password}");
			response.EnsureSuccessStatusCode();
			// Read the JWT token from the response
			return await response.Content.ReadAsStringAsync();

		}
		catch (Exception ex)
		{

		}

		return string.Empty;
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
}