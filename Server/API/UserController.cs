using System.Runtime.InteropServices.Marshalling;
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
    }

	/// <summary>
	/// Gets a user by their username
	/// </summary>
	/// <param name="Username">Username</param>
	/// <returns>Account Object</returns>
	[HttpGet("GetUserByID")]
	public string GetUserByUsername(string Username)
	{
	}
}