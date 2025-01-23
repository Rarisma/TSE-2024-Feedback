using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace Server;

public class AuthService 
{
	private readonly IConfiguration _configuration;
	private readonly TrackerContext _context;
	public AuthService(IConfiguration configuration, TrackerContext context)
	{
		_configuration = configuration;
		_context = context;
	}
	// Authenticates a user and returns a JWT token if successful
	public async Task<string> AuthenticateUserAsync(string Username, string password)
	{
		User? user = await _context.user.FirstOrDefaultAsync(u => u.Username == Username);
		if (user == null) { return null; } 

		// Verify password
		bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
		if (!isPasswordValid)
			return null; // Invalid password|

		//Update login time.
		user.LastLogin = DateTime.Now;
		_context.user.Update(user);

		// Generate JWT token
		string token = GenerateJwtToken(user);
		return token;
	}

	// Generates a JWT token for the authenticated user
	private string GenerateJwtToken(User user)
	{
		IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");
		string? secretKey = jwtSettings.GetValue<string>("SecretKey");
		string? issuer = jwtSettings.GetValue<string>("Issuer");
		string? audience = jwtSettings.GetValue<string>("Audience");
		int expiryMinutes = jwtSettings.GetValue<int>("ExpiryMinutes");

		SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));
		SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

		// Define claims
		Claim[] claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
			// Add additional claims if necessary
		};

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