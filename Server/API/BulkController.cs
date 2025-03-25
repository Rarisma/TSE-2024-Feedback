using System.Diagnostics.Contracts;
using Core.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Server.API;
/// <summary>
/// Used for bulk operations 
/// </summary>
[Route("bulk")]
public class BulkController
{
    /// <summary>
    /// Bulk sets teacher status
    /// </summary>
    /// <param name="teachersEmails">List of teacher Emails, separated by commas</param>
    /// <param name="isStudent">Value to Update</param>
    [HttpPost("SetTeacher")]
    public void SetTeacher(string teachersEmails, bool isStudent)
    {
        //Update every teacher
        using TrackerContext db = new();
        foreach (string teacher in teachersEmails.Split(","))
        {
            User? user = db.User.FirstOrDefault(t => t.Email == teacher);
            if (user != null)
            {
                user.IsTeacher = isStudent;
                db.User.Update(user);
                db.SaveChanges();
            }
        }
    }
    
    /// <summary>
    /// Creates a new users from a set of emails.
    /// Passwords default to the email address, users will be asked to update them on first login.
    /// </summary>
    /// <param name="userEmails">list of email addresses, separated by commas.</param>

    [HttpPost("CreateUsers")]
    public void CreateUsers(string userEmails)
    {
        using TrackerContext db = new();
        foreach (var email in userEmails.Split(","))
        {
            User user = new()
            {
                Email = email,
                Username = "TempStudent" + email,
                Initalised = false,
                IsStudent = false,
                Password = BCrypt.Net.BCrypt.HashPassword(email),
            };
            db.User.Add(user);
            db.SaveChanges();
        }
    }
    
    

    /// <summary>
    /// Creates new modules
    /// </summary>
    /// <param name="modules">names of modules, separated by commas</param>
    [HttpPost("CreateModules")]
    public void CreateModules(string modules)
    {
        using TrackerContext db = new();
        foreach (var module in modules.Split(","))
        {
            Modules mod = new(){Module=module};
            db.Modules.Add(mod);
            db.SaveChanges();
        }
    }

    
    /// <summary>
    /// Add users to module
    /// </summary>
    /// <param name="emails">User emails, separated by commas</param>
    /// <param name="moduleID">Module ID</param>
    [HttpPost("AssignModule")]
    public void AssignUserModule(string emails, int moduleID)
    {
        foreach (string email in emails.Split(","))
        {
            using TrackerContext db = new();
            User? user = db.User.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                Modules module = db.Modules.First(m => m.ModuleID == moduleID);
                db.UsersModules.Add(new Users_Modules{UserID = user.UserID, ModuleID = module.ModuleID});
                db.SaveChanges();
            }
        }
    }
}