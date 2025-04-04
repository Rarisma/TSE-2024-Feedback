using System.Text.Json;
using Core.Definitions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
    public string CreateModule([FromBody] Modules moduleObject)
    {
        try
        {
            Modules? module = moduleObject;
            if (module == null)
            {
                return "Invalid module object";
            }

            using TrackerContext ctx = new();
            ctx.Modules.Add(module);
            ctx.SaveChanges();

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
    /// <param name="id">Module ID</param>
    /// <return>Module Object</return>
    [HttpGet("GetModuleByID")]
    public string GetModuleByID(int id)
    {
        try
        {
            //Find account
            using TrackerContext ctx = new();
            Modules module = ctx.Modules.First(module => module.ModuleID == id);

			//Serialise to JSON
			return JsonSerializer.Serialize(module);
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }

	/// <summary>
	/// Get Module by Name
	/// </summary>
	/// <param name="name">Module ID</param>
	/// <return>Module Object</return>
	[HttpGet("GetModuleByName")]
    public string GetModuleByName(string name)
    {
        try
        {
            //Find account
            using TrackerContext ctx = new();
            Modules module = ctx.Modules.First(module => module.Module == name);

            //Serialise to JSON
            return JsonSerializer.Serialize(module);
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }

    /// <summary>
    /// Get Module by ID
    /// </summary>
    /// <param name="moduleID">Module ID Number</param>
    /// <return>Module Object</return>
    [HttpGet("GetUsersInModule")]
    public string GetUsersInModule(int moduleID)
    {
        try
        {
            //Find account
            using TrackerContext ctx = new();
            var users = (from Users_Modules usermodule in ctx.UsersModules
                         join userdata in ctx.User on usermodule.UserID equals userdata.UserID
                         where usermodule.ModuleID == moduleID
                         select new {
                             userdata.UserID,
                             userdata.Username,
                             userdata.Password,
                             userdata.IsStudent,
                             userdata.IsTeacher
                         }).ToList();

            //Serialise to JSON
            string json = JsonSerializer.Serialize(users);
            return json;
        }
        catch (Exception ex) { return "Encountered an error: " + ex.Message; }
    }

    /// <summary>
    /// Get Module by ID
    /// </summary>
    /// <param name="moduleID">Module ID Number</param>
    /// <return>Module Object</return>
    [HttpPost("AddUserToModule")]
    public async void AddUserToModule(int userId,int moduleID)
    {
        try
        {
            //Find account
            using TrackerContext ctx = new();

            var userMod = new Users_Modules() {ModuleID = moduleID,UserID = userId};

            ctx.UsersModules.Add(userMod);

            await ctx.SaveChangesAsync();
            Log.Debug("added user to module");
        }
        catch (Exception ex) { Log.Error(ex, "Failed to add user to module"); }
    }

    /// Remove Users from module
    [HttpPost("RemoveUserFromModule")]
    public async void RemoveUserFromModule(int userId,int moduleID)
    {
        try
        {
            using TrackerContext ctx = new();
            var userMod = ctx.UsersModules
                .Where(um => um.UserID == userId && um.ModuleID == moduleID)
                .FirstOrDefault();

            ctx.UsersModules.Remove(userMod);
            await ctx.SaveChangesAsync();
            Log.Debug("removed user from module");
        }
        catch (Exception ex) { Log.Error(ex, "Failed to remove user from module"); }

    }

    /// <summary>
    /// Gets all modules
    /// </summary>
    /// <returns>Every Module.</returns>
    [HttpGet("GetAllModules")]
    public async Task<string>? GetAllModules()
    {
        try
        {
            //Just get all modules.
            await using TrackerContext ctx = new();
            List<Modules>? modules =  ctx.Modules.ToList();
            return JsonSerializer.Serialize(modules);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to get all modules");
            throw;
        }
    }
}
