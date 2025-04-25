
using Core.Definitions;
using dotenv.net;
using Server;
using Server.API;
using System.Linq;


namespace ServerTests;

/// <summary>
/// Tests the feedback controller API.
/// </summary>
[TestClass]
public class UserControllerTests
{
    private readonly UserController _controller = new();

    //public UserControllerTests()
    //{
    //    // create authservice 
    //    AuthService authService = new();
    //    _controller = new UserController(authService);
    //}

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

    /// <summary>
    /// This hits the create feedback endpoint with an Invalid object.
    /// </summary>
    [TestMethod]
    public void CreateInvalidUser()
    {
        // Call function with invalid feedback
        _controller.CreateUser(null, null, null, null, null, null);

        // Assert our result is an error.
        Assert.Fail("Failed to create user");
    }







    [TestMethod]
    public void CheckInvalidUserID()
    {
        // Call function with invalid module name
        var result = _controller.GetUserById(-99999);

        // Assert our result is an error.
        Assert.IsTrue(result.Contains("Encountered an error"));
    }

    [TestMethod]
    public void CheckValidUserID()
    {
        User testAccount = new()
        {
            Username = "test",
            FirstName = "test",
            LastName = "test",
            Password = "test",
            IsTeacher = false,
            Email = "test",
            School = "test",
            Initalised = true,
            UserID = 1
        };

        // Create module
        _controller.CreateUser(testAccount.Username, testAccount.Password, testAccount.Email, testAccount.School, testAccount.FirstName, testAccount.LastName);

        // Get module 
        var result = _controller.GetUserById(testAccount.UserID);

        // Test if its the right module
        Assert.IsTrue(result.Contains(testAccount.UserID.ToString()), "Correct account ");

        // Clean up
        using TrackerContext ctx = new();

        //Clean up so we don't clutter the db.
        ctx.User.Remove(testAccount);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.User.Contains(testAccount), "user not cleaned up.");
    }

    [TestMethod]
    public void CheckInvalidUserName()
    {
        // Call function with invalid module name
        var result = _controller.GetUserByUsername(null);

        // Assert our result is an error.
        Assert.IsTrue(result.Contains("Encountered an error"));
    }

    [TestMethod]
    public void CheckValidModuleName()
    {
        User testAccount = new()
        {
            Username = "test",
            FirstName = "test",
            LastName = "test",
            Password = "test",
            IsTeacher = false,
            Email = "test",
            School = "test",
            Initalised = true,
            UserID = 1
        };

        // Create module
        _controller.CreateUser(testAccount.Username, testAccount.Password, testAccount.Email, testAccount.School, testAccount.FirstName, testAccount.LastName);

        // Get module 
        var result = _controller.GetUserByUsername(testAccount.Username);

        // Test if its the right module
        Assert.IsTrue(result.Contains(testAccount.Username), "Correct module");

        // Clean up
        using TrackerContext ctx = new();

        //Clean up so we don't clutter the db.
        ctx.User.Remove(testAccount);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.User.Contains(testAccount), "module not cleaned up.");
    }




































}

