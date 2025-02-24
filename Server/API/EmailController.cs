using Core.Definitions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Server.API
{
	[ApiController]
	[Route("Email")]
	public class EmailController : Controller
	{

		private readonly EmailSending emailSending;
		public EmailController(EmailSending emailSendingConstructor)
		{
			emailSending = emailSendingConstructor;
		}

		[HttpPost("SendEmail")]
		public async Task<IActionResult> SendEmail([FromBody] string ReceivingAddress)
		{
			using TrackerContext Ctx = new();
			User? Account = Ctx.User.FirstOrDefault(User => User.Email == ReceivingAddress);
			if (Account == null)
			{
				return StatusCode(400, "Failed to store code");
			}
			string EscapeEmail = Uri.EscapeDataString(ReceivingAddress);
			Random random = new Random();
			string Code = string.Empty;
			for (int i = 0; i < 6; i++)
			{
				string num = random.Next(0, 10).ToString();
				Code += num;
			}

			bool CodeStored = await AddCodeToDB(Code, ReceivingAddress);
			if (CodeStored == false)
			{
				return StatusCode(400, "Failed to store code");
			}
			string EmailBody = $@"
			<html>
			<body>
				<h1>Password Reset</h1>
				<p>Hi, you recently requested to change your password. Please click the link below to reset it:</p>
				<p>Please ensure you enter this code: <strong>{Code}</strong></p> 
            <p><a href=""https://localhost:7128/VerifyPasswordChange/email={EscapeEmail}"" target=""_blank"">Reset Password</a></p>
			</body>
			</html>";

			string EmailSubject = "Password Reset";

			if (string.IsNullOrEmpty(ReceivingAddress))
			{
				return StatusCode(400, "Email Address Missing");
			}

			await emailSending.SendEmailAsync(ReceivingAddress, EmailBody, EmailSubject);
			return StatusCode(200);
		}
        [HttpGet("AddCodeToDB")]
        public async Task<bool> AddCodeToDB(string Code, string Email)
		{
			try
			{
				using TrackerContext Ctx = new();
				User Account = Ctx.User.First(User => User.Email == Email);
				CodeStorage Details = new()
				{
					UserID = Account.UserID,
					CheckCode = Code,
				};
				Ctx.CodeStorage.Add(Details);
				await Ctx.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Failed to store reset code");
				return false;
			}
		}
		[HttpDelete("CheckAndDeleteCode")]
		public async Task<IActionResult> CheckAndDeleteCode(string InputtedCode)
		{
			using TrackerContext Ctx = new();
			CodeStorage? Storage = Ctx.CodeStorage.FirstOrDefault(CodeStorage => CodeStorage.CheckCode == InputtedCode);
			if (Storage == null)
			{
				return StatusCode(400, "Inputted Code Doesn't Match");
			}
			else
			{
				Ctx.CodeStorage.Remove(Storage);
				await Ctx.SaveChangesAsync();
				return StatusCode(200,"CODE DELETED");
			}
		}
	}
}