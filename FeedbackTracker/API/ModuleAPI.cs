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
    /// <param name="id">Module ID</param>
    public async Task<Modules?> GetModuleByID(int id)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Module/GetModuleByID?id={id}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Modules>(jsonString);
        }
        catch (Exception ex)
        {
			Log.Error(ex, $"Error getting module {id}");
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
            HttpResponseMessage response = await _httpClient.GetAsync(
                $"Module/GetModuleByName?name={Uri.EscapeDataString(name)}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Modules>(jsonString);
        }
        catch (Exception ex) {
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
            HttpResponseMessage response = await _httpClient.GetAsync(
                $"Module/GetUsersInModule?moduleID={Uri.EscapeDataString(moduleID.ToString())}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<User?>>(jsonString);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get users in module ");
            return null;
        }
    }

    /// <summary>
    /// Gets all modules that are available.
    /// </summary>
    /// <returns>List of modules.</returns>
    public async Task<List<Modules>> GetAllModules()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Module/GetAllModules");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Modules>>(jsonString) ?? new();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get all modules");
            return new();
        }
    }

    public async void AddUserToModule(string userid,string moduleid)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"Module/AddUserToModule?userID={Uri.EscapeDataString(userid.ToString())}&moduleID={Uri.EscapeDataString(moduleid.ToString())}",null);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to add user to module");
        }
    }

    public async void RemoveUserFromModule(string userid, string moduleid)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"Module/RemoveUserFromModule?userID={Uri.EscapeDataString(userid.ToString())}&moduleID={Uri.EscapeDataString(moduleid.ToString())}", null);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to remove user from module");
        }
    }

    public async Task<string> AddToModuleBySchool(string schoolid, string moduleid)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"Module/AddToModuleBySchool?schoolID={Uri.EscapeDataString(schoolid.ToString())}&moduleID={Uri.EscapeDataString(moduleid.ToString())}", null);
            response.EnsureSuccessStatusCode();
            return "success";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to add user to module");
            return "Error";
        }
    }
}
