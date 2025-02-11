using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FeedbackTrackerCommon.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Server;

public class AuthService(IConfiguration configuration, TrackerContext context)
{
	// Authenticates a user and returns a JWT token if successful
	public async Task<string?> AuthenticateUserAsync(string Username, string password)
	{
		User? user = await context.User.FirstOrDefaultAsync(u => u.Username == Username);
		if (user == null) {return null;} 

		// Verify password
		bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
		if (!isPasswordValid)
			return null; // Invalid password

		// Generate JWT token
		string token = GenerateJwtToken(user);
		return token;
	}

	/// <summary>
	/// Generates a Java Wev Token for a user
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private string GenerateJwtToken(User user)
	{
		//Get values from the configuration file
		IConfigurationSection jwtSettings = configuration.GetSection("JwtSettings");
		string? secretKey = jwtSettings.GetValue<string>("SecretKey");
		string? issuer = jwtSettings.GetValue<string>("Issuer");
		string? audience = jwtSettings.GetValue<string>("Audience");
		int expiryMinutes = jwtSettings.GetValue<int>("ExpiryMinutes");

		if (secretKey == null) { throw new Exception("Secret key is null"); }
		SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));
		SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

		// Define claims
		Claim[] claims =
		[
			new Claim(JwtRegisteredClaimNames.Sub, user.Username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
		];

		// Create the token
		JwtSecurityToken token = new(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}