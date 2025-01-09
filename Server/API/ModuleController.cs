using System.Text.Json;
using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;

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
}
