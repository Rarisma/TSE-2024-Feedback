using System.Text;
using System.Text.Json;
using Application.Components;
using Core.Definitions;
using Serilog;

namespace Application.API;

/// <summary>
/// Bulk API client
/// </summary>
public class NotificationAPI
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(App.Endpoint) };

    /// <summary>
    /// Deletes a feedback from the database.
    /// </summary>
    public async Task<string> DeleteFeedback(int NotificationID)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(
                $"Notification/delete?id={NotificationID}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Exception deleting feedback id={NotificationID}");
            return $"Encountered an error: {ex.Message}";
        }
    }


    /// <summary>
    /// Get notifications
    /// </summary>
    public async Task<List<Notification?>?> GetNotificationByUser(int user)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(
                $"notification/ByUser?userid={Uri.EscapeDataString(user.ToString())}");
            response.EnsureSuccessStatusCode();
            string jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Notification?>?>(jsonString);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error getting notification");
            return null;
        }
    }

    /// <summary>
    /// Add new notification
    /// </summary>
    /// <param name="userID">users id</param>
    /// <param name="title">title / header</param>
    /// <param name="message">main message</param>
    /// <param name="type">type of notifiaction (Feedback)</param>
    /// <param name="payload">additional data (feedback id for example)</param>
    public async Task NotificationPost(int userID, string title, string message, string type, string payload)
    {
        try
        {
            string url = $"notification?userid={Uri.EscapeDataString(userID.ToString())}" +
                         $"&Title={Uri.EscapeDataString(title.ToString())}" +
                         $"&Type={Uri.EscapeDataString(type.ToString())}" +
                         $"&Payload={Uri.EscapeDataString(payload.ToString())}" +
                         $"&Message={Uri.EscapeDataString(message.ToString())}";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error creating notification");
        }
    }

    /// <summary>
    /// Delete users notification store
    /// </summary>
    /// <param name="userID">users id</param>
    public async Task NotificationDelete(int userID)
    {
        try
        {
            string url = $"notification/ByUser?userID={Uri.EscapeDataString(userID.ToString())}";
            using var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error Deleting notification");
        }
    }

}
