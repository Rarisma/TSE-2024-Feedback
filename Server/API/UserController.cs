using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
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
	/// <param name="ID">Account ID</param>
	/// <returns>Account Object</returns>
	[HttpGet("GetUserByID")]
	public string GetUserByID(int ID)
	{
		try
		{
			//Find account
			using TrackerContext Ctx = new();
			User Account = Ctx.User.First(User => User.UserID == ID);

			//Serialise to JSON
			string json = JsonSerializer.Serialize(Account);
			return json;
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}

	/// <summary>
	/// Gets a user by their username
	/// </summary>
	/// <param name="Username">Username</param>
	/// <returns>Account Object</returns>
	[HttpGet("GetUserByUsername")]
	public string GetUserByUsername(string Username)
	{
		try
		{
			//Find account
			using TrackerContext Ctx = new();
			User Account = Ctx.User.First(User => User.Username == Username);

			//Serialise to JSON
			return JsonSerializer.Serialize(Account);
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}

	/// <summary>
	/// Creates a new user object.
	/// </summary>
	/// <param name="Username">Account username</param>
	/// <param name="Password">Account password (in plaintext)</param>
	/// <returns></returns>
	[HttpPost("CreateUser")]
	public async void CreateUser(string Username, string Password, string Email)
	{
		try
		{
			//Create account object
			//NOTE: bCrypt is very secure. (Salting is handled automatically)
			User Account = new()
			{
				Username = Username,
				Password = BCrypt.Net.BCrypt.HashPassword(Password),
				IsStudent = true,
				IsTeacher = false,
				Email = Email,
			};


			//Add user to database
			await using TrackerContext Ctx = new();
			Ctx.User.Add(Account);
			await Ctx.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to create user");
		}
	}

	/// <summary>
	/// Authenticates a user
	/// </summary>
	/// <param name="Username">User's account username</param>
	/// <param name="Password">account password</param>
	/// <param name="Password">account MFA Code</param>
	/// <returns></returns>
	[HttpGet("Authenticate")]
	public async Task<string?> Authenticate(string Username, string Password, string Code)
	{
		return await authService.AuthenticateUserAsync(Username, Password, Code);
	}



	[HttpGet("GetUsers")]
	public Task<string> GetUsers()
	{
		try
		{
			//Find accounts

			using TrackerContext Ctx = new();
			List<User> Accounts = Ctx.User.ToList();
			var result = Accounts.ToList();

			Accounts.ForEach(acc => acc.Password = "");
			return Task.FromResult(JsonSerializer.Serialize(result));
		}
		catch (Exception ex) {
			return null;
		}
	}


	/// <summary>
	/// Creates a new user object.
	/// </summary>
	/// <param name="Username">Account username</param>
	/// <param name="Password">Account password (in plaintext)</param>
	/// <returns></returns>
	[HttpGet("GetModules")]
	public string GetModules(int Userid)
	{
		try
		{
            //Find account
            using TrackerContext Ctx = new();
            var modules = (from Users_Modules usermodule in Ctx.UsersModules
                         join moduledata in Ctx.Modules on usermodule.ModuleID equals moduledata.ModuleID
                         where usermodule.UserID == Userid
                         select new
                         {
                             ModuleID = moduledata.ModuleID,
							 Module = moduledata.Module,

                         }).ToList();

            //Serialise to JSON
            string json = JsonSerializer.Serialize(modules);
            return json;
        }
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}
	[HttpGet("CreateTOTPKey")]
	public async Task<StatusCodeResult> CreateTOTPKey(string UserID, string Password)
	{
		try
		{
			using TrackerContext Ctx = new();
			User Account = Ctx.User.First(User => User.UserID == Convert.ToInt32(UserID));

			if (Account.MFASecret != null)
			{
				Log.Warning("User already has 2FA");
				return StatusCode(405);
			}

			//prevent totp from being added where it shouldnt be.
			if (Account.Password != Password)
			{
				Log.Warning("Invalid auth");
				return StatusCode(401);
			}
			
			Log.Information("adding mfa for accounts without mfa");
			var secret = KeyGeneration.GenerateRandomKey(20);
			Account.MFASecret = Base32Encoding.ToString(secret);
			Ctx.User.Update(Account);
			await Ctx.SaveChangesAsync();
			return StatusCode(200);
		}
		catch (Exception ex)
		{
			Log.Warning("Error occured during adding MFA");
			return StatusCode(500);
		}
	}
	
	[HttpGet("MFABool")]
	public bool getMFAStatus(int UserID)
	{
		try
		{
			//Find account
			using TrackerContext Ctx = new();
			User Account = Ctx.User.First(User => User.UserID == UserID);
			return !(string.IsNullOrEmpty(Account.MFASecret));
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Unexpected error when getting MFA status for account: " + UserID);
			return false;
		}
	}
	
	    /// <summary>
    /// Creates a new user object.
    /// </summary>
    /// <param Userid="user id">Account user id</param>
    /// <returns></returns>
    [HttpGet("Notification")]
    public string NotificationGet(int Userid)
    {
        try
        {
            //Find account
            using TrackerContext Ctx = new();
            var notifications = (from Notification notificaiton in Ctx.Notification
                                 where notificaiton.UserID == Userid
                           select new
                           {
                               NotificationID = notificaiton.NotificationID,
                               UserID = notificaiton.UserID,
                               FeedbackID = notificaiton.FeedbackID,
                               Timestamp = notificaiton.Timestamp,

                           }).ToList();

            //Serialise to JSON
            string json = JsonSerializer.Serialize(notifications);
            return json;
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }

    /// <summary>
    /// Creates a new user object.
    /// </summary>
    /// <param Userid="user id">Account user id</param>
    /// <returns></returns>
    [HttpPost("Notification")]
    public async void NotificationPost(int Userid, int FeedbackID)
    {
        try
        {
			//Create account object
			//NOTE: bCrypt is very secure. (Salting is handled automatically)
			Notification notification = new()
			{
				UserID = Userid,
				FeedbackID = FeedbackID,
				Timestamp = DateTime.Now,
			};


            //Add user to database
            await using TrackerContext Ctx = new();
            Ctx.Notification.Add(notification);
            await Ctx.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create user");
        }
    }

    /// <summary>
    /// Creates a new user object.
    /// </summary>
    /// <param Userid="user id">Account user id</param>
    /// <returns></returns>
    [HttpDelete("Notification")]
    public async void NotificationDelete(int Userid)
    {
        try
        {
			//Add user to database
            await using TrackerContext Ctx = new();
            Ctx.Notification.RemoveRange(Ctx.Notification.Where(notification => notification.UserID == Userid));
            await Ctx.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create user");
        }
    }

	[HttpPut("UpdatePassword")]
	public async void UpdatePassword(string Email, string Password)
	{
		try
		{
			using TrackerContext Ctx = new();
			User Account = Ctx.User.First(User => User.Email == Email);
			Account.Password = BCrypt.Net.BCrypt.HashPassword(Password);
			Ctx.User.Update(Account);
			await Ctx.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to update password");
		}
	}
}