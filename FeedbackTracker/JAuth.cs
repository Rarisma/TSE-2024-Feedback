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
public class JAuth(NavigationManager NavigationManager)
{
	public User? User;
	private ClaimsPrincipal? UserData;
	private string? JWT;
	
	/// <summary>
	/// Authorises a user.
	/// </summary>
	/// <param name="token"></param>
	public async void Authorise(string token, string user)
	{
		string secretKey = "oLiaKsJ93IrvBF0nrIYTdhdR8X+o7tZfHq9ITBVpUew=";
		string issuer = "www.hallon.rarisma.net:5002";
		string audience = "www.hallon.rarisma.net:5003";

		//Configure validation parameter
		TokenValidationParameters validationParameters = new()
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
			ValidateIssuer = true,
			ValidIssuer = issuer,
			ValidateAudience = true,
			ValidAudience = audience,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.FromMinutes(5)
		};

		JwtSecurityTokenHandler tokenHandler = new();

		try
		{
			// Validate the token
			UserData = tokenHandler.ValidateToken(token,
				validationParameters, out SecurityToken validatedToken);

			User = await new UserAPI().GetUserByUsername(user);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred during token validation: {ex.Message}");
		}
	}

	public bool IsAuthorised()
	{
		//Check UserData.Identity is set.
		if (UserData?.Identity == null)
		{
			return false;
		}

		return UserData.Identity.IsAuthenticated;
	}

	/// <summary>
	/// De-Authorises the session, logs the user out.
	/// </summary>
	public void Deauthorise()
	{
		User = new();
		JWT = "";
		UserData = null;
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
				NavigationManager.NavigateTo("/LogIn");
			}
			Log.Information("User authorised.");
			return;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error enforcing authentication");
		}
		
		//Send to login since user unauthorised
		try
		{
			NavigationManager.NavigateTo("/LogIn");
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

	public string? GetToken() => JWT;

}