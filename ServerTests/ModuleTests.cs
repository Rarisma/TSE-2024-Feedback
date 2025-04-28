
using Core.Definitions;
using dotenv.net;
using Server;
using Server.API;

namespace ServerTests;

/// <summary>
/// Tests the Module controller API.
/// </summary>
[TestClass]
public class ModuleControllerTests
{
    private readonly ModuleController _controller = new();



    [TestMethod]
    public void CreateInvalidModule()
    {
        // Call function with invalid feedback
        var result = _controller.CreateModule(null);

        // Assert our result is an error.
        Assert.IsTrue(result.Contains("Invalid module object"));
    }


    [TestMethod]
    public void CreateValidModule()
    {
        Modules testModule = new Modules
        {
            Module = Guid.NewGuid().ToString(),
            ModuleID = 1,
        };

        // Call function with valid module
        _controller.CreateModule(testModule);

        // Assert the database has an error
        using TrackerContext ctx = new();
        Assert.IsTrue(ctx.Modules.Contains(testModule), "Database doesn't contain test module");

        //Clean up so we don't clutter the db.
        ctx.Modules.Remove(testModule);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.Modules.Contains(testModule), "module not cleaned up.");
    }

    /// <summary>
    /// Checks if the feedback ID of a Feedback is invalid
    /// </summary>
    [TestMethod]
    public void CheckInvalidModuleID()
    {
        // Call function with invalid module ID
        var result = _controller.GetModuleByID(-9999);

        // Assert our result is an error.
        Assert.IsFalse(result.Contains("Invalid module ID"));
    }


    /// <summary>
    /// This hits the GetFeedbackById  endpoint
    /// </summary>
    [TestMethod]
    public void CheckValidModuleID()
    {
        Modules testModule = new Modules
        {
            Module = Guid.NewGuid().ToString(),
            ModuleID = 1,
        };

        // Create module
        _controller.CreateModule(testModule);

        // Get module 
        var result = _controller.GetModuleByID(testModule.ModuleID);

        // Test if its the right module
        Assert.IsTrue(result.Contains(testModule.Module), "Correct module ID");

        // Clean up
        using TrackerContext ctx = new();

        //Clean up so we don't clutter the db.
        ctx.Modules.Remove(testModule);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.Modules.Contains(testModule), "module not cleaned up.");
    }

    [TestMethod]
    public void CheckInvalidNameModule()
    {
        // Call function with invalid module name
        var result = _controller.GetModuleByName(null);

        // Assert our result is an error.
        Assert.IsFalse(result.Contains("Invalid module name"));
    }

    [TestMethod]
    public void CheckValidModuleName()
    {
        Modules testModule = new Modules
        {
            Module = Guid.NewGuid().ToString(),
            ModuleID = 1,
        };

        // Create module
        _controller.CreateModule(testModule);

        // Get module 
        var result = _controller.GetModuleByName(testModule.Module);

        // Test if its the right module
        Assert.IsTrue(result.Contains(testModule.Module), "Correct module");

        // Clean up
        using TrackerContext ctx = new();

        //Clean up so we don't clutter the db.
        ctx.Modules.Remove(testModule);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.Modules.Contains(testModule), "module not cleaned up.");
    }
}




