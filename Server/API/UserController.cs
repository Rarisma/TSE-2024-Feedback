using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;
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
	public async void CreateUser(string Username, string Password)
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
	/// <returns></returns>
	[HttpGet("Authenticate")]
	public async Task<string?> Authenticate(string Username, string Password)
	{
		return await authService.AuthenticateUserAsync(Username, Password);
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
            var modules = (from UsersModules usermodule in Ctx.UsersModules
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

}