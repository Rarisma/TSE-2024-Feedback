using System.Net;
using System.Net.Mail;

namespace Server;

public class EmailSending
{
	private const string TseEmail = "tsefeedback9@gmail.com";
	private const string EmailPassword = "uunx kzkn yimy whmd";

	public static async Task SendEmailAsync(string ReceivingAddress, string body, string subject)
	{
		var Client = new SmtpClient("smtp.gmail.com", 587)
		{
			EnableSsl = true,
			Credentials = new NetworkCredential(TseEmail, EmailPassword)
		};

		try
		{
			var Email = new MailMessage
			{
				From = new MailAddress(TseEmail),
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			Email.To.Add(ReceivingAddress);

			await Client.SendMailAsync(Email);
		}
		catch ( Exception ex )
		{
			Console.WriteLine($"Error during transmission: {ex.Message}");
			Console.WriteLine(ex.StackTrace);
		}
	}
}