using FeedbackTrackerCommon.Definitions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;


namespace Server.API;
[ApiController]
[Route("School")]
public class SchoolController : Controller
{

    [HttpPost("CreateSchool")]
    public string CreateSchool(string SchoolName, string EducationLevel, string City)
    {
        try
        {
            School School = new()
            {
                SchoolName = SchoolName,
                EducationLevel = EducationLevel,
                City = City,
            };

            // Add School to database
            using TrackerContext Ctx = new();
            Ctx.School.Add(School);
            Ctx.SaveChanges();


            return "School Created Successfully";
        }

        catch (Exception ex)
        {
            return "Encountered an error: " + ex.Message;
        }
    }

    [HttpGet("GetSchoolByName")]
    public string GetSchoolByName(string name)
    {
        try
        {
            // Find School
            using TrackerContext ctx = new();
            School school = ctx.School.First(School => School.SchoolName == name);

            return JsonSerializer.Serialize(school);

        }

        catch (Exception ex) { return "Encountered an error: " + ex.Message; }







    }
}


