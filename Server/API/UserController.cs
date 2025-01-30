using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
			Ctx.user.Add(Account);
			await Ctx.SaveChangesAsync();
			await Ctx.DisposeAsync();
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
	/// Gets modules for user
	/// </summary>
	/// <param name="Userid">Account ID</param>
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

	[HttpGet("CreateTOTPKey")]
	public string CreateTOTPKey()
	{
		var secret  = KeyGeneration.GenerateRandomKey(20);
		string base32Secret = Base32Encoding.ToString(secret);
		return base32Secret;

	}

	[HttpGet("VerifyTOTPKey")]
	public bool VerifyTOTPKey(string Secret, string Code)
	{
		var base32Bytes = Base32Encoding.ToBytes(Secret);
		var totp = new Totp(base32Bytes, mode: OtpHashMode.Sha1);
		string Verify = totp.ComputeTotp();
		return Code == Verify;
	}
}