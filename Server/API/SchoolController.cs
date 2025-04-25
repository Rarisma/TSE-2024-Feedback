using Core.Definitions;
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
            try
            {
                // Add school module
                var module = new Modules { Module = SchoolName + " General" };
                Ctx.Modules.Add(module);
            }

            catch (Exception ex)
            {
                return "Encountered an error in module creation: " + ex.Message;
            }

            // Add new teacher user for school

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

    [HttpGet("GetAllSchools")]
    public string GetAllSchools()
    {
        try
        {
            // Find School
            using TrackerContext ctx = new();
            List<School> school = ctx.School.ToList();

            return JsonSerializer.Serialize(school);

        }

        catch (Exception ex) { return "Encountered an error: " + ex.Message; }


    }
}


