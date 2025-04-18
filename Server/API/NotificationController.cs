using System.Diagnostics.Contracts;
using System.Text.Json;
using Core.Definitions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Server.API;
/// <summary>
/// Use this controller for notifications
/// </summary>
[Route("notification")]
 public class NotificationController
 {

    /// <summary>
    /// deletes a notification
    /// </summary>
    /// <param name="id">notification</param>
    [HttpDelete("delete")]
    public string NotificationDelete(int id)
    {
        try
        {

            //get db context
            using TrackerContext ctx = new();
            //get notification
            var noti = ctx.Notification.Where(n => n.NotificationID == id);
            //remove
            ctx.Remove(noti);
            //save
            ctx.SaveChanges();

            return "notification removed successfully";
        }
        catch (Exception ex)
        {
            return "Encountered an error: " + ex.Message;
        }
    }

    /// <summary>
    /// Creates a new user object.
    /// </summary>
    /// <param name="userid">Account user id</param>
    /// <returns></returns>
    [HttpGet("ByUser")]
    public string NotificationGetByUser(int userid)
    {
        try
        {
            //Find account
            using TrackerContext ctx = new();
            var notifications = (from Notification notification in ctx.Notification
                                 where notification.UserID == userid
                                 select new
                                 {
                                     notification.NotificationID,
                                     notification.UserID,
                                     notification.Title,
                                     notification.Message,
                                     notification.TimeSinceCreation,
                                     notification.Timestamp,
                                     notification.Type,
                                     notification.Payload
                                 }).ToList();

            //Serialise to JSON
            return JsonSerializer.Serialize(notifications);
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }

    /// <summary>
    /// Creates a new notifification object.
    /// </summary>
    /// <param name="userid">Account user id</param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPost("/")]
    public async void NotificationPost(int userid, string title, string message, string type, string payload)
    {
        try
        {
            //Create notification object
            Notification notification = new()
            {
                UserID = userid,
                // remove feedback after merge
                FeedbackId = 156,
                Title = title,
                Message = message,
                Timestamp = DateTime.Now,
                Type = type,
                Payload = payload

            };


            //Add user to database
            await using TrackerContext ctx = new();
            ctx.Notification.Add(notification);
            await ctx.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create notification");
        }
    }

    /// <summary>
    /// Creates a new user object.
    /// </summary>
    /// <param name="userID">account id</param>
    /// <returns></returns>
    [HttpDelete("ByUser")]
    public async void NotificationDeleteByUser(int userID)
    {
        try
        {
            //Add user to database
            await using TrackerContext ctx = new();
            ctx.Notification.RemoveRange(ctx.Notification
                .Where(notification => notification.UserID == userID));
            await ctx.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete notification");
        }
    }

}

