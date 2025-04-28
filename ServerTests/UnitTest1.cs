
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

    [ClassInitialize]
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
            FeedbackID = 1,
            FeedbackText = "TestDescription",
            Label = FeedbackLabel.Question,
            AssigneeID = 1,
            AssignedUserID = 1,
            Closed = false,
            ModuleID = 1,
            
        };
        
        // Call function with valid feedback
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


    /// <summary>
    /// Checks if the feedback ID of a Feedback is invalid
    /// </summary>
    [TestMethod]
    public void CheckInvalidFeebackID()
    {
        // Call function with invalid feedback ID
        var result = _controller.GetFeedbackByID(-9999);

        // Assert our result is an error.
        Assert.IsFalse(result.Contains("Invalid feedback ID"));
    }



    /// <summary>
    /// This hits the GetFeedbackById  endpoint
    /// </summary>
    [TestMethod]
    public void CheckValidFeebackID()
    {
        Feedback testFeedback = new Feedback
        {
            Title = Guid.NewGuid().ToString(), //GUIDs are globally unique
            FeedbackID = 1,
            FeedbackText = "TestDescription",
            Label = FeedbackLabel.Question,
            AssigneeID = 1,
            AssignedUserID = 1,
            Closed = false,
            ModuleID = 1,

        };

        // Create Feedback
        _controller.CreateFeedback(testFeedback);

        // Get feedback 
        var result = _controller.GetFeedbackByID(testFeedback.FeedbackID);

        // Test if its the right feedback
        Assert.IsTrue(result.Contains(testFeedback.FeedbackID.ToString()), "Feedback doesnt include the ID.");

        // Clean up
        using TrackerContext ctx = new();

        //Clean up so we don't clutter the db.
        ctx.Feedback.Remove(testFeedback);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.Feedback.Contains(testFeedback), "Feedback not cleaned up.");
    }

    /// <summary>
    /// Checks if the feedback ID of a Feedback is invalid
    /// </summary>
    [TestMethod]
    public void GetPublicFeedbacksInvalid()
    {
        // Call function with Public feedbacks
        var result = _controller.GetPublicFeedbacks();

        // Assert our result is an error.
        Assert.IsFalse(result.Contains("Invalid feedback ID"));
    }

    /// <summary>
    /// Tests feedback visibility
    /// </summary>
    [TestMethod]
    public void TestFeebackVisibility()
    {
        Feedback publicfeedback = new Feedback
        {
            Title = Guid.NewGuid().ToString(), //GUIDs are globally unique
            FeedbackText = "TestDescription",
            Label = FeedbackLabel.Question,
            AssigneeID = 1,
            AssignedUserID = 1,
            Closed = false,
            ModuleID = 1,
            Visibility = FeedbackVisibility.Public

        };
        Feedback privatefeedback = new Feedback
        {
            Title = Guid.NewGuid().ToString(), //GUIDs are globally unique
            FeedbackText = "TestDescription2",
            Label = FeedbackLabel.Question,
            AssigneeID = 2,
            AssignedUserID = 2,
            Closed = false,
            ModuleID = 1,
            Visibility = FeedbackVisibility.Private

        };


        // Create Feedback
        _controller.CreateFeedback(publicfeedback);
        _controller.CreateFeedback(privatefeedback);



        // Call function with valid feedback
        string result = _controller.GetPublicFeedbacks();

        // Tests if it only finds public feedbacks
        Assert.IsFalse(result.Contains(publicfeedback.Title), "Contains public feedback");
        Assert.IsFalse(result.Contains(privatefeedback.Title), "Doesnt contain private feedback");

        //Clean up so we don't clutter the db.
        using TrackerContext ctx = new();

        ctx.Feedback.Remove(publicfeedback);
        ctx.Feedback.Remove(privatefeedback);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.Feedback.Contains(publicfeedback), "Feedback not cleaned up.");
        Assert.IsFalse(ctx.Feedback.Contains(privatefeedback), "Feedback not cleaned up.");

    }


    [TestMethod]
    public void InvalidComment()
    {
        // Call function with invalid create comment
        var result = _controller.CreateComment(-1,1,"comment");

        // Assert our result is an error.
        Assert.IsFalse(result.Contains("Invalid comment"));
    }

    [TestMethod]
    public void CheckValidComment()
    {
        Feedback testFeedback = new Feedback
        {
            Title = Guid.NewGuid().ToString(), //GUIDs are globally unique
            FeedbackID = 999,
            FeedbackText = "TestDescription",
            Label = FeedbackLabel.Question,
            AssigneeID = 1,
            AssignedUserID = 1,
            Closed = false,
            ModuleID = 1,
            Visibility = FeedbackVisibility.Public

        };

        // Create Feedback
        _controller.CreateFeedback(testFeedback);

        var commentText = _controller.CreateComment(999, 1, "comment");

        // Get comment 
        var result = _controller.GetComments(1);

        // Test if its the right feedback
        Assert.IsFalse(result.Contains(commentText), "Feedback doesnt' contains comment");

        // Clean up
        using TrackerContext ctx = new();

        //Clean up so we don't clutter the db.
        ctx.Feedback.Remove(testFeedback);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.Feedback.Contains(testFeedback), "Feedback not cleaned up.");
    }

}
