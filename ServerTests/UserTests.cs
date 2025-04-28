
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
    private readonly UserController _controller;

    public UserControllerTests()
    {
        var context = new TrackerContext();
        var authService = new AuthService(null, context);
        _controller = new UserController(authService);
    }


    /// <summary>
    /// Sets up the db.
    /// </summary>
    [ClassInitialize]
    public static void Setup(TestContext context)
    {
        //Tracker context (DB access) needs secrets.
        //This is usually initialised when server.program.main() is ran but we don't want that.
        DotEnv.Load();
        Program.Secrets = DotEnv.Read();
    }


    /// <summary>
    /// This hits the create user endpoint with an Invalid object.
    /// </summary>
    [TestMethod]
    public void CreateInvalidUser()
    {
        // Call function with invalid user
        try
        {
            _controller.CreateUser(null, null, null, null, null, null);

            Assert.Fail("cannot create user");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex, typeof(AssertFailedException), "failed to create a user");
        }
    }
    


    [TestMethod]
    public void CheckInvalidUserID()
    {
        // Call function with invalid user id
        var result = _controller.GetUserById(-99999);

        // Assert our result is an error.
        Assert.IsTrue(result.Contains("Encountered an error"));
    }

    [TestMethod]
    public void CheckValidUserID()
    {
        User testAccount = new()
        {
            Username = "UniqueUnitTestingUser",
            FirstName = "test",
            LastName = "test",
            Password = "test",
            IsTeacher = false,
            Email = "test",
            School = "test",
            Initalised = true,
        };

        // Create user
        _controller.CreateUser(testAccount.Username, testAccount.Password, testAccount.Email, testAccount.School, testAccount.FirstName, testAccount.LastName);

        using TrackerContext ctx = new();

        var createdUser = ctx.User.FirstOrDefault(s => s.Username == "UniqueUnitTestingUser");


        // Get module 
        var result = _controller.GetUserById(createdUser.UserID);

        // Test if its the right user
        Assert.IsTrue(result.Contains(createdUser.UserID.ToString()), "Correct account ");


        //Clean up so we don't clutter the db.

        ctx.User.Remove(createdUser);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.User.Contains(createdUser), "user not cleaned up.");
    }

    [TestMethod]
    public void CheckInvalidUserName()
    {
        // Call function with invalid user name
        var result = _controller.GetUserByUsername(null);

        // Assert our result is an error.
        Assert.IsTrue(result.Contains("Encountered an error"));
    }

    [TestMethod]
    public void CheckValidUserName()
    {
        User testAccount = new()
        {
            Username = "UnitTestingUser",
            FirstName = "test",
            LastName = "test",
            Password = "test",
            IsTeacher = false,
            Email = "test",
            School = "test",
            Initalised = true,
        };

        // Create user
        _controller.CreateUser(testAccount.Username, testAccount.Password, testAccount.Email, testAccount.School, testAccount.FirstName, testAccount.LastName);

        // Get user 
        var result = _controller.GetUserByUsername(testAccount.Username);

        // Test if its the right user
        Assert.IsTrue(result.Contains(testAccount.Username), "Correct user");

        // Clean up
        using TrackerContext ctx = new();



        //Clean up so we don't clutter the db.
        var DeleteUser = ctx.User.FirstOrDefault(s => s.Username == "UnitTestingUser");


            ctx.User.Remove(DeleteUser);
            ctx.SaveChanges();

            //Assert our result is a success.
            Assert.IsFalse(ctx.User.Contains(testAccount), "module not cleaned up.");
        }
}







































