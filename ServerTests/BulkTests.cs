using Core.Definitions;
using Server;
using Server.API;
using dotenv.net;
namespace ServerTests;

/// <summary>
/// Tests the Module controller API.
/// </summary>
[TestClass]
public class BulkControllerTests
{
    private readonly BulkController _controller = new();

    /// <summary>
    /// Sets up the db.
    /// </summary>
    [AssemblyInitialize]
    public static void Setup(TestContext context)
    {
        //Tracker context (DB access) needs secrets.
        //This is usually initialised when server.program.main() is ran but we don't want that.
        DotEnv.Load();
        Program.Secrets = DotEnv.Read();
    }

    [TestMethod]
    public void CreateInvalidUsers()
    {
        // Call function with invalid feedback
        _controller.CreateUsers(null);

        // Assert our result is an error.
        Assert.Fail("Failed to create user");
    }

    



}


