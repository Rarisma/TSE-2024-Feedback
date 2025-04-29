
using Core.Definitions;
using dotenv.net;
using Server;
using Server.API;


namespace ServerTests;

/// <summary>
/// Tests the school controller API.
/// </summary>
[TestClass]
public class SchoolControllerTests
{
    private readonly SchoolController _controller = new();

    [ClassInitialize]
    public static void Setup(TestContext context)
    {
        //Tracker context (DB access) needs secrets.
        //This is usually initialised when server.program.main() is ran but we don't want that.
        DotEnv.Load();
        Program.Secrets = DotEnv.Read();
    }


    [TestMethod]
    public void CreateValidSchool()
    {
        School testSchool = new()
        {
            SchoolName = "testschoolunitest",
            EducationLevel = "test",
            City = "test",
        };

        // create school
        _controller.CreateSchool(testSchool.SchoolName, testSchool.EducationLevel, testSchool.City);

        using TrackerContext ctx = new();

        // assert
        Assert.IsFalse(ctx.School.Contains(testSchool), "module not cleaned up.");


        //Clean up so we don't clutter the db.

        var DeleteSchool = ctx.School.FirstOrDefault(s => s.SchoolName == "testschoolunitest");

        ctx.School.Remove(DeleteSchool);
        ctx.SaveChanges();


        //Assert our result is a success.
        Assert.IsFalse(ctx.School.Contains(testSchool), "module not cleaned up.");
    }


    [TestMethod]
    public void CheckInvalidSchoolName()
    {
        // Call function with invalid school name
        var result = _controller.GetSchoolByName(null);

        // Assert our result is an error.
        Assert.IsFalse(result.Contains("Invalid school name"));
    }


    [TestMethod]
    public void CheckValidSchoolName()
    {
        School testSchool = new()
        {
            Id = 999,
            SchoolName = "testschoolunitest",
            EducationLevel = "test",
            City = "test",
        };

        // Create school
        _controller.CreateSchool(testSchool.SchoolName, testSchool.EducationLevel, testSchool.City);

        // Get school 
        var result = _controller.GetSchoolByName(testSchool.SchoolName);

        // Test if its the right module
        Assert.IsTrue(result.Contains(testSchool.SchoolName), "Incorrect School");

        // Clean up
        using TrackerContext ctx = new();

        var DeleteSchool = ctx.School.FirstOrDefault(s => s.SchoolName == "testschoolunitest");

        //Clean up so we don't clutter the db.
        ctx.School.Remove(DeleteSchool);
        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.School.Contains(testSchool), "school not cleaned up.");
    }


    [TestMethod]
    public void InvalidGetAllSchools()
    {
        // Call function with invalid school name
        var result = _controller.GetAllSchools();

        // Assert our result is an error.
        Assert.IsFalse(result.Contains("no schools created"));
    }






    [TestMethod]
    public void ValidGetallschools()
    {
        School testSchool1 = new()
        {
            SchoolName = "testschool1",
            EducationLevel = "test",
            City = "test",
        };

        School testSchool2 = new()
        {
            SchoolName = "testschool2",
            EducationLevel = "test",
            City = "test",
        };

        // create schools
        _controller.CreateSchool(testSchool1.SchoolName, testSchool1.EducationLevel, testSchool1.City);
        _controller.CreateSchool(testSchool2.SchoolName, testSchool2.EducationLevel, testSchool2.City);

        // call get all schools function
        var result = _controller.GetAllSchools();

        Assert.IsTrue(result.Contains(testSchool1.SchoolName), "Contains first school");
        Assert.IsTrue(result.Contains(testSchool2.SchoolName), "Contains second school");


        // Clean up
        using TrackerContext ctx = new();

        //Clean up so we don't clutter the db.
        var DeleteSchool1 = ctx.School.FirstOrDefault(s => s.SchoolName == "testschool1");
        var DeleteSchool2 = ctx.School.FirstOrDefault(s => s.SchoolName == "testschool2");


        ctx.School.Remove(DeleteSchool1);
        ctx.School.Remove(DeleteSchool2);

        ctx.SaveChanges();

        //Assert our result is a success.
        Assert.IsFalse(ctx.School.Contains(testSchool1), "school1 not cleaned up.");
        Assert.IsFalse(ctx.School.Contains(testSchool2), "school2 not cleaned up.");
    }
}