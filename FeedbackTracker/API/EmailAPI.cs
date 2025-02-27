using Application.Components;
using Serilog;

namespace Application.API;

public class EmailAPI
{
	private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(App.Endpoint) };

	public async Task<bool> SendEmail(string? receivingAddress)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Email/SendEmail", receivingAddress);
			if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				return false;
			}
			response.EnsureSuccessStatusCode();
			return true;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to send email");
			return false;
		}
	}

	public async Task<bool> CheckAndDeleteCode(string inputtedCode)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.DeleteAsync(
				$"Email/CheckAndDeleteCode?InputtedCode={inputtedCode}");
			response.EnsureSuccessStatusCode();
			return true;
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to update code");
			return false;
		}
	}
}

