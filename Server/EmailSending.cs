using System.Net;
using System.Net.Mail;

namespace Server;

public class EmailSending
{
	public static async Task SendEmailAsync(string receivingAddress, string body, string subject)
	{
		var Client = new SmtpClient("smtp.gmail.com", 587)
		{
			EnableSsl = true,
			Credentials = new NetworkCredential(Program.Secrets["ResetEmail"], 
				Program.Secrets["ResetEmail"])
		};

		try
		{
			var Email = new MailMessage
			{
				From = new MailAddress(Program.Secrets["ResetEmailPassword"]),
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			Email.To.Add(receivingAddress);

			await Client.SendMailAsync(Email);
		}
		catch ( Exception ex )
		{
			Console.WriteLine($"Error during transmission: {ex.Message}");
			Console.WriteLine(ex.StackTrace);
		}
	}
}