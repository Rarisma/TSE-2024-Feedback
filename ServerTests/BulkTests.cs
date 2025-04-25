using Core.Definitions;
using Server;
using Server.API;
using dotenv.net;
namespace ServerTests;

/// <summary>
/// Tests the Module
/// </summary>
[TestClass]
public class BulkControllerTests
{
    private readonly BulkController _controller = new();

    [TestMethod]
    public void CreateInvalidUsers()
    {
        // Call function with invalid feedback
        _controller.CreateUsers(null);

        // Assert our result is an error.
        Assert.Fail("Failed to create user");
    }
}