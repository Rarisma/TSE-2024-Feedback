using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OtpNet;

namespace Server;

public class AuthService(IConfiguration configuration, TrackerContext context)
{
	/// <summary>
	/// Handles authentication 
	/// </summary>
	/// <param name="username">Accounts username</param>
	/// <param name="password">Accounts password</param>
	/// <param name="TOTP">Account MFA code </param>
	/// <returns></returns>
	public async Task<string?> AuthenticateUserAsync(string username, string password, string TOTP = "0")
	{
		User? user = await context.User.FirstOrDefaultAsync(u => u.Username == username);
		if (user == null) {return null;} 

		// Verify password
		bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
		if (!isPasswordValid)
			return null; // Invalid password

		//Verify MFA Code
		if (user.MFASecret != null)
		{
			byte[]? base32Bytes = Base32Encoding.ToBytes(user.MFASecret);
			var totp = new Totp(base32Bytes, mode: OtpHashMode.Sha1);
			string verify = totp.ComputeTotp();

			//Invalid TOTP code, unauthorised.
			if (verify != TOTP)
			{
				return null;
			}
		}
		
		//Update login time.
		user.LastLogin = DateTime.Now;
		context.User.Update(user);

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
			new(JwtRegisteredClaimNames.Sub, user.Username),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new(ClaimTypes.NameIdentifier, user.UserID.ToString())
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