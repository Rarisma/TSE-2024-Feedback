using Microsoft.AspNetCore.Mvc;

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

		[HttpPost]
		public async Task<IActionResult> SendEmail([FromBody] string ReceivingAddress)
		{
			string EscapeEmail = Uri.EscapeDataString(ReceivingAddress);

			string EmailBody = @"
				<html>
				<body>
					<h1>Password Reset</h1>
					<p>Hi, you recently requested to change your password. Please click the link below to reset it:</p>
					<p><a href=""https://localhost:7128/VerifyPasswordChange/email=" + EscapeEmail + @""" target=""_blank"">Reset Password</a></p>
				</body>
				</html>";

			string EmailSubject = "Password Reset";

			if (string.IsNullOrEmpty(ReceivingAddress))
			{
				return StatusCode(400);
			}
			
			await emailSending.SendEmailAsync(ReceivingAddress, EmailBody, EmailSubject);
			return StatusCode(200);
		}
	}
}