using System.Text.Json;
using System.Text.Json.Serialization;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;


namespace Server.API;
[ApiController]
[Route("Module")]
public class ModuleController : Controller
{
    /// <summary>
    /// Get Module by ID
    /// </summary>
    /// <param name="ID">Module ID</param>
    /// <return>Module Object</return>
    [HttpGet("GetModuleByID")]
    public string GetModuleByID(int ID)
    {
        try
        {
            //Find account
            using TrackerContext Ctx = new();
            Modules module = Ctx.modules.First(module => module.ModuleID == ID);

            //Serialise to JSON
            string json = JsonSerializer.Serialize(module);
            return json;
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }

    /// <summary>
    /// Get Module by ID
    /// </summary>
    /// <param name="ID">Module ID</param>
    /// <return>Module Object</return>
    [HttpGet("GetModuleByName")]
    public string GetModuleByName(string Name)
    {
        try
        {
            //Find account
            using TrackerContext Ctx = new();
            Modules module = Ctx.modules.First(module => module.Module == Name);

            //Serialise to JSON
            string json = JsonSerializer.Serialize(module);
            return json;
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }

    /// <summary>
    /// Get Module by ID
    /// </summary>
    /// <param id="ID">Module ID</param>
    /// <return>Module Object</return>
    [HttpGet("GetUsersInModule")]
    public string GetUsersInModule(int ModuleID)
    {
        try
        {
            //Find account
            using TrackerContext Ctx = new();
            var users = (from UsersModules usermodule in Ctx.users_modules
                         join userdata in Ctx.user on usermodule.UserID equals userdata.UserID
                         where usermodule.ModuleID == ModuleID
                         select new {
                             UserID = userdata.UserID,
                             Username = userdata.Username,
                             Password = userdata.Password,
                             IsStudent = userdata.IsStudent,
                             IsTeacher = userdata.IsTeacher
                         
                         }).ToList();

            //Serialise to JSON
            string json = JsonSerializer.Serialize(users);
            return json;
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }
}
