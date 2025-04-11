using Core.Definitions;
using dotenv.net;
using Server;
using Server.API;

namespace ServerTests;

/// <summary>
/// Tests the feedback controller API.
/// </summary>
[TestClass]
public class FeedbackControllerTests
{
    private readonly FeedbackController _controller = new();

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
    public void CreateInvalidFeedback()
    {
        // Call function with invalid feedback
        var result = _controller.CreateFeedback(null);
        
        // Assert our result is an error.
        Assert.IsTrue(result.Contains("Invalid feedback object"));
    }


    /// <summary>
    /// This hits the create feedback endpoint with a valid object.
    /// </summary>
    [TestMethod]
    public void CreateValidFeedback()
    {
        Feedback testFeedback = new Feedback
        {
            Title = Guid.NewGuid().ToString(), //GUIDs are globally unique
            FeedbackText = "TestDescription",
            Label = FeedbackLabel.Question,
            AssigneeID = 1,
            AssignedUserID = 1,
            Closed = false,
            ModuleID = 1,
            
        };
        
        // Call function with invalid feedback
        _controller.CreateFeedback(testFeedback);
        
        // Assert the database has an error
        using TrackerContext ctx = new();
        Assert.IsTrue(ctx.Feedback.Contains(testFeedback), "Database doesn't contain test feedback");
        
        //Clean up so we don't clutter the db.
        ctx.Feedback.Remove(testFeedback);
        ctx.SaveChanges();
        
        //Assert our result is a success.
        Assert.IsFalse(ctx.Feedback.Contains(testFeedback), "Feedback not cleaned up.");
    }
}