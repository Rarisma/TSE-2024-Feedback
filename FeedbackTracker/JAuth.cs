using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.API;
using Core.Definitions;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Application;
/// <summary>
/// JAuth (JWTAuth, or JakeAuth) is a simple solution to blazor and razor
/// authentication methods, because if you can't get the .NET version of it
/// to work, you might as well be on your own.
/// </summary>
public class JAuth(NavigationManager navigationManager)
{
	public User? User;
	private ClaimsPrincipal? _userData;
	private string? JWT;

	/// <summary>
	/// Authorises a user.
	/// </summary>
	/// <param name="token">Account token</param>
	/// <param name="user">Username</param>
	public async Task Authorise(string token, string user)
	{
		//Configure validation params
		if (Program.JWTSecretKey != null)
		{
			TokenValidationParameters validationParameters = new()
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.JWTSecretKey)),
				ValidateIssuer = true,
				ValidIssuer = Program.JWTIssuer,
				ValidateAudience = true,
				ValidAudience = Program.JWTAudience,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.FromMinutes(5)
			};

			JwtSecurityTokenHandler tokenHandler = new();

			try
			{
				// Validate the token
				_userData = tokenHandler.ValidateToken(token,
					validationParameters, out SecurityToken _);

				User = await new UserAPI().GetUserByUsername(user);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred during token validation: {ex.Message}");
			}
		}
	}

	public bool IsAuthorised()
	{
		//Check UserData.Identity is set.
		if (_userData?.Identity == null)
		{
			return false;
		}

		return _userData.Identity.IsAuthenticated;
	}

	/// <summary>
	/// De-Authorises the session, logs the user out.
	/// </summary>
	public void Deauthorise()
	{
		User = new();
		JWT = "";
		_userData = null;
	}

	/// <summary>
	/// Checks if a page is logged in, if not redirects to /unauthorised
	/// You should call this on the page initalizer.
	/// See the Remarks section of this function for how to
	/// use this function correctly.
	/// </summary>
	/// <remarks>
	/// Sample usage:
	///	protected override async Task OnInitializedAsync()	{
	///		JAuth.EnforceAuth();
	///	}
	/// </remarks>
	public void EnforceAuth()
	{
		Log.Information("Checking user information...");
		try
		{
			if (!IsAuthorised())
			{
				Log.Information("User unauthorised.");
				navigationManager.NavigateTo("/login");
			}
			Log.Information("User authorised.");
			return;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error enforcing authentication");
		}
		
		//Send to log in since user unauthorised
		try
		{
			navigationManager.NavigateTo("/LogIn");
		}
		catch (Exception ex)
		{
            Log.Error(ex, "Error redirecting to login");
        }
	}

	/// <summary>
	/// Helper function to get User object.
	/// </summary>
	public User? GetUser() => User;

	/// <summary>
	/// Gets the users JWT.
	/// </summary>
	/// <returns>JWT of current user if any.</returns>
	public string? GetToken() => JWT;

}