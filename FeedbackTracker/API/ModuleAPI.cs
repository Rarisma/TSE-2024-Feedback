using System.Text;
using System.Text.Json;
using Application.Components;
using Core.Definitions;
using Serilog;

namespace Application.API;

public class ModuleAPI
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(App.Endpoint) };

    /// <summary>
    /// Creates a new module.
    /// </summary>
    public async Task<string?> CreateModule(Modules module)
    {
        try
        {
            string jsonContent = JsonSerializer.Serialize(module);
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("Module/CreateModule", content);
            response.EnsureSuccessStatusCode();
            // Read response content
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating module");
            return null;
        }
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
			Log.Error(ex, $"Error getting module {ID}");
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
	        Log.Error(ex, $"Error getting module name {name}");
			return null;
        }
    }

    /// <summary>
    /// return users in module
    /// </summary>
    public async Task<List<User?>?> GetUsersInModule(int moduleID)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Module/GetUsersInModule?ModuleID={Uri.EscapeDataString(moduleID.ToString())}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<User?>>(jsonString);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

}
