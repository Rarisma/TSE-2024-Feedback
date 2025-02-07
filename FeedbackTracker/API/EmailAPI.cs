namespace Application.API;

public class EmailAPI (string baseEndpoint)
{
	private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(baseEndpoint) };

	public async Task<bool> SendEmail(string ReceivingAddress)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Email", ReceivingAddress);
			response.EnsureSuccessStatusCode();
			return true;
		}
		catch (Exception ex)
		{
			return false;
		}
	}
}

