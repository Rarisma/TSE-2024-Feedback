using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Serilog;

namespace Application.API;

public class UserAPI(string baseEndpoint)
{
	private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(baseEndpoint) };

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
	public async Task CreateUser(string Username, string Password)
	{
		try
		{
			string url = $"User/CreateUser?Username={Uri.EscapeDataString(Username)}" +
						 $"&Password={Uri.EscapeDataString(Password)}";
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
			Log.Error(ex, $"Authenticating user {Username}");
			return string.Empty;
		}
	}
}