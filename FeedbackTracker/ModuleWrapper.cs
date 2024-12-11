using System.Text;
using System.Text.Json;
using FeedbackTrackerCommon.Definitions;

namespace Application;

public class ModuleAPI
{
    private readonly HttpClient _httpClient;

    public ModuleAPI(string baseEndpoint)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseEndpoint) };
    }

    /// <summary>
    /// Gets a module by ID
    /// </summary>
    /// <param name="ID">Module ID</param>
    public async Task<Modules?> GetModuleByID(int ID)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Module/GetModuleByID?ID={ID}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Modules>(jsonString);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// get module by name.
    /// </summary>
    public async Task<Modules?> GetModuleByName(string name)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Module/GetModuleByName?Name={Uri.EscapeDataString(name)}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Modules>(jsonString);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

}
