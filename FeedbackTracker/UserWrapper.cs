using System.Text;
using System.Text.Json;
using FeedbackTrackerCommon.Definitions;

namespace Application;

public class UserAPI
{
	private readonly HttpClient _httpClient;
	private readonly IHttpContextAccessor HttpContext;

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


	/// <summary>
	/// Authenticates a user.
	/// </summary>
	/// <param name="Username">Username</param>
	/// <param name="Password">Password</param>
	/// <returns>True if authentication is successful, otherwise, false.</returns>
	public async Task<bool> Authenticate(string Username, string Password)
	{
		try
		{
			// Send login request.
			HttpResponseMessage response = await _httpClient.GetAsync($"User/Authenticate?Username={Username}&Password={Password}");
			response.EnsureSuccessStatusCode();

			// Read the JWT token from the response
			string token = await response.Content.ReadAsStringAsync();

			if (!string.IsNullOrEmpty(token))
			{
				// Set the JWT as an HTTP-only cookie
				var httpContext = HttpContext.HttpContext;
				if (httpContext != null)
				{
					httpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
					{
						HttpOnly = true,
						SameSite = SameSiteMode.Strict,
						Secure = true, // Ensure HTTPS
						Expires = DateTimeOffset.UtcNow.AddMinutes(600)
					});
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			// Log the exception (ex) as needed
		}

		return false;
	}
}