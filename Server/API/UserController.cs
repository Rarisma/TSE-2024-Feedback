using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Server.API;

[Route("User")]
public class UserController : Controller
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
			User Account = Ctx.Users.First(User => User.UserID == ID);

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
			User Account = Ctx.Users.First(User => User.Username == Username);

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
	public async string CreateUser(string Username, string Password)
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
			Ctx.Users.Add(Account);
			Ctx.SaveChanges();
			await Ctx.DisposeAsync();
			return "User created successfully";
		}
		catch (Exception ex) { return "Encountered an error: " + ex.Message; }
	}
}