using System.Text.Json;
using Core.Definitions;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using Serilog;

namespace Server.API;
[ApiController]
[Route("User")]
public class UserController(AuthService authService) : Controller
{
	/// <summary>
	/// Gets a user by their User ID
	/// </summary>
	/// <param name="id">Account ID</param>
	/// <returns>Account Object</returns>
	[HttpGet("GetUserByID")]
	public string GetUserById(int id)
	{
		try
		{
			//Find account
			using TrackerContext ctx = new();
			User account = ctx.User.First(user => user.UserID == id);

			//Serialise to JSON
			string json = JsonSerializer.Serialize(account);
			return json;
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}

	/// <summary>
	/// Gets a user by their username
	/// </summary>
	/// <param name="username">Username</param>
	/// <returns>Account Object</returns>
	[HttpGet("GetUserByUsername")]
	public string GetUserByUsername(string username)
	{
		try
		{
			//Find account
			using TrackerContext ctx = new();
			User account = ctx.User.First(user => user.Username == username);

			//Serialise to JSON
			return JsonSerializer.Serialize(account);
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}

	/// <summary>
	/// Creates a new user object.
	/// </summary>
	/// <param name="username">Account username</param>
	/// <param name="password">Account password (in plaintext)</param>
	/// <param name="email">Account email (in plaintext)</param>
	/// <returns></returns>
	[HttpPost("CreateUser")]
	public async void CreateUser(string Username, string Password, string Email, string School,string FirstName, string LastName)
	{
		try
		{
			//Create account object
			//NOTE: bCrypt is very secure. (Salting is handled automatically)
			User account = new()
			{
				Username = Username,
				FirstName = FirstName,
				LastName = LastName,
				Password = BCrypt.Net.BCrypt.HashPassword(Password),
				IsTeacher = false,
				Email = Email,
				School = School,
				Initalised = true
			};
			
			//Add user to database
			await using TrackerContext ctx = new();
			ctx.User.Add(account);
			await ctx.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to create user");
		}
	}

	/// <summary>
	/// Authenticates a user
	/// </summary>
	/// <param name="username">User's account username</param>
	/// <param name="password">account password</param>
	/// <param name="code">account MFA Code</param>
	/// <returns></returns>
	[HttpGet("Authenticate")]
	public async Task<string?> Authenticate(string username, string password, string code = "")
	{
		return await authService.AuthenticateUserAsync(username, password, code);
	}

	[HttpGet("GetAllUsers")]
	public Task<string>? GetAllUsers()
	{
		try
		{
			//Find accounts
			using TrackerContext ctx = new();
			List<User> accounts = ctx.User.ToList();
			var result = accounts.ToList();

			accounts.ForEach(acc => acc.Password = "");
			return Task.FromResult(JsonSerializer.Serialize(result));
		}
		catch (Exception ex) {
			Log.Error(ex, "Error getting users");
			return null;
		}
	}


	/// <summary>
	/// Gets all modules
	/// </summary>
	/// <returns>All modules</returns>
	[HttpGet("GetModules")]
	public string GetModules(int userid)
	{
		try
		{
            //Find account
            using TrackerContext ctx = new();
            var modules = (from Users_Modules usermodule in ctx.UsersModules
                         join moduledata in ctx.Modules on usermodule.ModuleID equals moduledata.ModuleID
                         where usermodule.UserID == userid
                         select new { moduledata.ModuleID, moduledata.Module, }).ToList();

            //Serialise to JSON
            return JsonSerializer.Serialize(modules);
        }
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}
	
	[HttpGet("CreateTOTPKey")]
	public async Task<StatusCodeResult> CreateTotpKey(string userId, string password)
	{
		try
		{
			await using TrackerContext ctx = new();
			User account = ctx.User.First(user => user.UserID == Convert.ToInt32(userId));

			if (account.MFASecret != null)
			{
				Log.Warning("User already has 2FA");
				return StatusCode(405);
			}

			//prevent totp from being added where it shouldn't be.
			if (account.Password != password)
			{
				Log.Warning("Invalid auth");
				return StatusCode(401);
			}
			
			Log.Information("adding mfa for accounts without mfa");
			byte[]? secret = KeyGeneration.GenerateRandomKey(20);
			account.MFASecret = Base32Encoding.ToString(secret);
			ctx.User.Update(account);
			await ctx.SaveChangesAsync();
			return StatusCode(200);
		}
		catch (Exception ex)
		{
			Log.Warning(ex, "Error occured during adding MFA");
			return StatusCode(500);
		}
	}
	
	[HttpGet("GetMfaStatus")]
	public bool GetMfaStatus(int userId)
	{
		try
		{
			//Find account
			using TrackerContext ctx = new();
			User account = ctx.User.First(user => user.UserID == userId);
			return !string.IsNullOrEmpty(account.MFASecret);
		}
		catch (Exception ex)
		{
			Log.Error(ex, $"Unexpected error when getting MFA status for account: {userId}" );
			return false;
		}
	}
	

	[HttpPut("UpdatePassword")]
	public async void UpdatePassword(string email, string password)
	{
		try
		{
			await using TrackerContext ctx = new();
			User account = ctx.User.First(user => user.Email == email);
			account.Password = BCrypt.Net.BCrypt.HashPassword(password);
			ctx.User.Update(account);
			await ctx.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to update password");
		}
	}

    [HttpPut("UpdateUser")]
    public async void UpdateUser(string UserID,string FirstName, string LastName, string Username, string Password)
    {
        try
        {
            await using TrackerContext ctx = new();
            User account = ctx.User.First(user => user.UserID.ToString() == UserID);

			account.Username = Username;
			account.FirstName = FirstName;
			account.LastName = LastName;
            account.Password = BCrypt.Net.BCrypt.HashPassword(Password);

            ctx.User.Update(account);
            await ctx.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update password");
        }
    }
    /// <summary>
    /// update teacher status
    /// </summary>
    /// <param name="userid"></param>
	/// <param name="role"></param>
    [HttpPut("RoleUpdate")]
    public async void RoleUpdate(int userid, bool role)
    {
        try
        {
            await using TrackerContext ctx = new();
            User account = ctx.User.First(user => user.UserID == userid);
            account.IsTeacher = role;
            ctx.User.Update(account);
            await ctx.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to update role");
        }
    }

    /// <summary>
    /// Gets an average resolve time for a teacher
    /// </summary>
    /// <param name="userId"></param>
    [HttpGet("GetAvgResolveTime")]
	public float GetAverageResolveTime(int userId)
	{
		using TrackerContext ctx = new();
		User account = ctx.User.First(user => user.UserID == userId);
		if (account.IsTeacher)
		{
			//TODO: Check with fin if assign all in module is null
			//TODO: Check with team or mark if avg time should include assign all.
			var feedbacks = ctx.Feedback.Where(f => (f.AssignedUserID == userId ||
			                                        f.AssignedUserID == null)& f.Closed) .ToList(); 
			var total = TimeSpan.Zero;
			feedbacks.ForEach(feedback => total += (feedback.ClosedDate - feedback.CreatedDate)!.Value);
			return (float)total.TotalHours / feedbacks.Count;
		}

		return 0;
	}
}