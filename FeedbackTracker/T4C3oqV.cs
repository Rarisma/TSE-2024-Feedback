using System.Text;
using System.Text.Json;
using FeedbackTrackerCommon.Definitions;

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
	public async Task<string> CreateUser(User user)
	{
		try
		{
			//Serialise
			string jsonContent = JsonSerializer.Serialize(user);
			StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

			HttpResponseMessage response = await _httpClient.PostAsync("User/CreateUser", content);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			return $"Error: {ex.Message}";
		}
	}
}