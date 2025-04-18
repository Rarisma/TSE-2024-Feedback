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
			HttpResponseMessage response = await _httpClient.GetAsync(
				$"User/GetUserByID?id={id}");
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
			var response = await _httpClient.GetAsync(
				$"User/GetUserByUsername?username={Uri.EscapeDataString(username)}");
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

	public async Task CreateUser(string Username, string Password, string Email, string School,string FirstName,string LastName)
	{
		try
		{
			string url = $"User/CreateUser?Username={Uri.EscapeDataString(Username)}" +
						 $"&Password={Uri.EscapeDataString(Password)}" +
                         $"&School={Uri.EscapeDataString(School)}" +
                         $"&LastName={Uri.EscapeDataString(LastName)}" +
                         $"&FirstName={Uri.EscapeDataString(FirstName)}" +
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
	/// <param name="username">Username</param>
	/// <param name="password">Password</param>
	/// <param name="code">MFA Code (Optional)</param>
    /// <returns>Boolean indicating success.</returns>
	public async Task<string> Authenticate(string username, string password, string code)
	{
		try
		{
			// Send login request.
			HttpResponseMessage response = await _httpClient.GetAsync(
				$"User/Authenticate?username={Uri.EscapeDataString(username)}" +
				$"&password={Uri.EscapeDataString(password)}&code={code}");

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
    /// Enables 2FA for account
    /// </summary>
    /// <param name="userID">Account ID to enable 2FA for</param>
    /// <param name="password">Password for verification</param>
    /// <returns>Boolean indicating success.</returns>
    public async Task<bool> CreateTotpKey(string userID, string password)
    {
	    try
	    {
		    var endpoint = $"User/CreateTOTPKey?userId={Uri.EscapeDataString(userID)}&password={Uri.EscapeDataString(password)}";
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
		    var endpoint = $"User/GetMfaStatus?userId={userID}";
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
	/// Updates user password
	/// </summary>
	/// <param name="email">Account email</param>
	/// <param name="password">Account password</param>
	public async Task UpdatePassword(string email, string password)
	{
		try
		{
			string url = $"User/UpdatePassword?email={Uri.EscapeDataString(email)}" +
			 $"&password={Uri.EscapeDataString(password)}";
			HttpResponseMessage response = await _httpClient.PutAsync(url, null);
			response.EnsureSuccessStatusCode();
			await response.Content.ReadAsStringAsync();

		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Error updating password");
		}
	}

	
	public async Task<List<User>?> GetAllUsers()
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync("User/GetAllUsers");
			response.EnsureSuccessStatusCode();
			string jsonString = await response.Content.ReadAsStringAsync();
			var users = JsonSerializer.Deserialize<List<User>?>(jsonString);
			return await Task.FromResult(users);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Exception getting users");
			return null;
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
				$"User/GetModules?userid={Uri.EscapeDataString(user.ToString())}");
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



    public async Task<string> getAverageResolveTime(int UserID)
    {
        try
        {
            string url = $"User/GetAvgResolveTime?Userid={Uri.EscapeDataString(UserID.ToString())}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string resp = await response.Content.ReadAsStringAsync();
			return resp;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error getiing resolve time");
			return null;
        }
    }


}