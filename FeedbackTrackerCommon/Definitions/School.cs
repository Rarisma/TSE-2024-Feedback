using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;
    public class School
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("SchoolName")]
    public string SchoolName { get; set; }

    [Column("Education_Level")]
    public string EducationLevel { get; set; }

    [Column("City")]
    public string City { get; set; }
}