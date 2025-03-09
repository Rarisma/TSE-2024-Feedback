using Core.Definitions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Server.API
{
	[ApiController]
	[Route("Email")]
	public class EmailController : Controller
	{

		[HttpPost("SendEmail")]
		public async Task<IActionResult> SendEmail([FromBody] string? receivingAddress)
		{
			await using TrackerContext ctx = new();
			User? account = ctx.User.FirstOrDefault(user => user.Email == receivingAddress);
			if (account == null)
			{
				return StatusCode(400, "Failed to store code");
			}
			string escapeEmail = Uri.EscapeDataString(receivingAddress);
			Random random = new();
			string code = string.Empty;
			for (int i = 0; i < 6; i++)
			{
				string num = random.Next(0, 10).ToString();
				code += num;
			}

			bool codeStored = await AddCodeToDb(code, receivingAddress);
			if (codeStored == false)
			{
				return StatusCode(400, "Failed to store code");
			}
			string emailBody = $"""
			                    
			                        <html>
			                        <body>
			                            <h1>Password Reset</h1>
			                            <p>Hi, you recently requested to change your password. Please click the link below to reset it:</p>
			                            <p>Please ensure you enter this code: <strong>{code}</strong></p> 
			                        <p><a href="https://localhost:7128/VerifyPasswordChange/email={escapeEmail}" target="_blank">Reset Password</a></p>
			                        </body>
			                        </html>
			                    """;
			
			if (string.IsNullOrEmpty(receivingAddress))
			{
				return StatusCode(400, "Email Address Missing");
			}

			await EmailSending.SendEmailAsync(receivingAddress, emailBody, "Password Reset");
			return StatusCode(200);
		}
        [HttpGet("AddCodeToDB")]
        public async Task<bool> AddCodeToDb(string code, string email)
		{
			try
			{
				await using TrackerContext ctx = new();
				User account = ctx.User.First(user => user.Email == email);
				CodeStorage details = new()
				{
					UserID = account.UserID,
					CheckCode = code,
				};
				ctx.CodeStorage.Add(details);
				await ctx.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Failed to store reset code");
				return false;
			}
		}
		[HttpDelete("CheckAndDeleteCode")]
		public async Task<IActionResult> CheckAndDeleteCode(string inputtedCode)
		{
			await using TrackerContext ctx = new();
			CodeStorage? storage = ctx.CodeStorage.FirstOrDefault(codeStorage =>
				codeStorage.CheckCode == inputtedCode);
			if (storage == null)
			{
				return StatusCode(400, "Inputted Code Doesn't Match");
			}
			else
			{
				ctx.CodeStorage.Remove(storage);
				await ctx.SaveChangesAsync();
				return StatusCode(200,"CODE DELETED");
			}
		}
	}
}