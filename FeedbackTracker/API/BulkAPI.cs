using System.Text.Json;
using Application.Components;
using Serilog;

namespace Application.API;

/// <summary>
/// Bulk API client
/// </summary>
public class BulkAPI
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(App.Endpoint) };

    /// <summary>
    /// Sets users as teachers or not.
    /// </summary>
    public async Task SetTeacher(string teachersEmails, bool isStudent)
    {
        try
        {
            string url = $"bulk/SetTeacher?teachersEmails={Uri.EscapeDataString(teachersEmails)}&isStudent={isStudent}";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting teacher status");
        }
    }

    /// <summary>
    /// Creates multiple users from email list.
    /// </summary>
    public async Task CreateUsers(string userEmails)
    {
        try
        {
            string url = $"bulk/CreateUsers?userEmails={Uri.EscapeDataString(userEmails)}";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating users");
        }
    }

    /// <summary>
    /// Creates multiple modules.
    /// </summary>
    public async Task CreateModules(string modules)
    {
        try
        {
            string url = $"bulk/CreateModules?modules={Uri.EscapeDataString(modules)}";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating modules");
        }
    }

    /// <summary>
    /// Assigns users to a module.
    /// </summary>
    public async Task AssignUserModule(string emails, int moduleID)
    {
        try
        {
            string url = $"bulk/AssignModule?emails={Uri.EscapeDataString(emails)}&moduleID={moduleID}";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error assigning users to module {moduleID}");
        }
    }
}
