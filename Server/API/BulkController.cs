using Core.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Server.API;

[Route("bulk")]
public class BulkController
{
    /// <summary>
    /// Bulk sets teacher status
    /// </summary>
    /// <param name="teachers">List of teacher Emails</param>
    /// <param name="isStudent">Value to Update</param>
    [HttpPost("SetTeacher")]
    public void SetTeacher(List<string> teachers, bool isStudent)
    {
        //Update every teacher
        using TrackerContext db = new();
        foreach (var teacher in teachers)
        {
            var user = db.User.First(t => t.Email == teacher);
            user.IsTeacher = isStudent;
            db.User.Update(user);
            db.SaveChanges();
        }
    }

    /// <summary>
    /// Creates new modules
    /// </summary>
    /// <param name="modules">names of modules</param>
    [HttpPost("CreateModule")]
    public void CreateModules(List<string> modules)
    {
        using TrackerContext db = new();
        foreach (var module in modules)
        {
            Core.Definitions.Modules mod = new(){Module=module};
            db.Modules.Add(mod);
            db.SaveChanges();
        }
    }

    
    /// <summary>
    /// Add users to module
    /// </summary>
    /// <param name="emails">User emails</param>
    /// <param name="moduleID">Module ID</param>
    [HttpPost("AssignModule")]
    public void AssignUserModule(List<string> emails, int moduleID)
    {
        foreach (var email in emails)
        {
            using TrackerContext db = new();
            User user = db.User.First(u => u.Email == email);
            Modules module = db.Modules.First(m => m.ModuleID == moduleID);
            db.UsersModules.Add(new Users_Modules{UserID = user.UserID, ModuleID = module.ModuleID});
            db.SaveChanges();
        }
    }
}