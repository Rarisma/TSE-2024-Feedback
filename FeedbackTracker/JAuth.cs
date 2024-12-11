using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

namespace Application;
/// <summary>
/// JAuth (JWTAuth, or JakeAuth) is a simple solution to blazor and razor
/// authentication methods, because if you can't get the .NET version of it
/// to work, you might as well be on your own.
/// </summary>
public class JAuth
{
	public static User User;
	private static ClaimsPrincipal? UserData;
	private NavigationManager Navi;
	private static string JWT;

	public JAuth(NavigationManager NavigationManager)
	{
		
		Navi = NavigationManager;
	}

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

			User = await new UserAPI("http://localhost:5189").GetUserByUsername(user);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred during token validation: {ex.Message}");
		}
	}

	public bool IsAuthorised()
	{
		if (UserData == null)
		{
			return false;
		}
		if (UserData.Identity.IsAuthenticated)
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// De-Authorises the session, logs the user out.
	/// </summary>
	/// <returns></returns>
	public void Deauthorise()
	{
		User = null;
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
		try
		{
			if (UserData == null)
			{
				Navi.NavigateTo("/Unauthorised");
			}
			if (UserData.Identity.IsAuthenticated)
			{
				return;
			}
		}
		catch
		{

		}
		Navi.NavigateTo("/Unauthorised");

	}
}