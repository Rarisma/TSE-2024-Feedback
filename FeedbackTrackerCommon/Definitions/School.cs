using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;
    public class School
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("SCHOOL_NAME")]
    public string SchoolName { get; set; }

    [Column("EDUCATION_LEVEL")]
    public string EducationLevel { get; set; }

    [Column("CITY")]
    public string City { get; set; }
}