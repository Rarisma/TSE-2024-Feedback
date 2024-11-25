using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Server.API;
[ApiController]
[Route("User")]
public class UserController : Controller
{
	private readonly AuthService _authService;

	public UserController(AuthService authService)
	{
		_authService = authService;
	}
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
			User Account = Ctx.user.First(User => User.UserID == ID);

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
			User Account = Ctx.user.First(User => User.Username == Username);

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
	[HttpGet("CreateUser")]
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
			using TrackerContext Ctx = new();
			Ctx.user.Add(Account);
			Ctx.SaveChanges();
			await Ctx.DisposeAsync();
		}
		catch (Exception ex) {  }
	}

	/// <summary>
	/// Authenticates a user
	/// </summary>
	/// <param name="Username">User's account username</param>
	/// <param name="Password">account password</param>
	/// <returns></returns>
	[HttpGet("Authenticate")]
	public Task<string> Authenticate(string Username, string Password)
	{
		return _authService.AuthenticateUserAsync(Username, Password);
	}
}