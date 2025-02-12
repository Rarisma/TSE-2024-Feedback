namespace Application.API;

public class EmailAPI (string baseEndpoint)
{
	private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(baseEndpoint) };

	public async Task<bool> SendEmail(string ReceivingAddress)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Email/SendEmail", ReceivingAddress);
			if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				return false;
			}
			response.EnsureSuccessStatusCode();
			return true;
		}
		catch (Exception ex)
		{
			return false;
		}
	}

	public async Task<bool> CheckAndDeleteCode(string InputtedCode)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.DeleteAsync($"Email/CheckAndDeleteCode?InputtedCode={InputtedCode}");
			response.EnsureSuccessStatusCode();
			return true;
		}
		catch (Exception ex)
		{
			return false;
		}
	}
}

