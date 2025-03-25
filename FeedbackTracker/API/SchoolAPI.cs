using System.Text;
using System.Text.Json;
using Application.Components;
using Application.Components.Pages;
using Core.Definitions;
using Serilog;
using static System.Net.Mime.MediaTypeNames;

namespace Application.API;



public class SchoolAPI
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(App.Endpoint) };

    // Create School
    public async Task<string> CreateSchool(string SchoolName, string EducationLevel, string City)
    {
        try
        {
            //Format for server
            StringContent SchoolData = new StringContent($"\"{SchoolName}\"", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"School/CreateSchool", SchoolData);


            //Send
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception creating school");
            return $"Encountered an error: {ex.Message}";
        }
    }


    // Gets school names
    public async Task<List<School>?> GetAllSchools()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("School/GetAllSchools");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<School>>(jsonString);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting school names");
            return null;
        }
    }
}