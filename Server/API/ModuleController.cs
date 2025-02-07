using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Server.API;
[ApiController]
[Route("Module")]
public class ModuleController : Controller
{

    /// <summary>
    /// Creates new module
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateModule")]
    public string CreateModule([FromBody] Modules ModuleObject)
    {
        try
        {
            Modules? Module = ModuleObject;
            if (Module == null)
            {
                return "Invalid module object";
            }

            using TrackerContext Ctx = new();
            Ctx.Modules.Add(Module);
            Ctx.SaveChanges();

            return "Module created successfully";
        }
        catch (Exception ex)
        {
            return "Encountered an error: " + ex.Message;
        }
    }



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
            Modules module = Ctx.Modules.First(module => module.ModuleID == ID);

			//Serialise to JSON
			return JsonSerializer.Serialize(module); ;
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }

	/// <summary>
	/// Get Module by Name
	/// </summary>
	/// <param name="Name">Module ID</param>
	/// <return>Module Object</return>
	[HttpGet("GetModuleByName")]
    public string GetModuleByName(string Name)
    {
        try
        {
            //Find account
            using TrackerContext Ctx = new();
            Modules module = Ctx.Modules.First(module => module.Module == Name);

            //Serialise to JSON
            return JsonSerializer.Serialize(module); ;
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
            var users = (from Users_Modules usermodule in Ctx.Users_Modules
                         join userdata in Ctx.User on usermodule.UserID equals userdata.UserID
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
