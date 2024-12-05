using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedbackTrackerCommon.Definitions;

public class Extension
{
    ///<summary>
    /// Extension ID
    /// </summary>
    [Key]
    [Column("EXTENSION_ID")]
    public int ExtensionId { get; set; }

    ///<sumary>
    ///feedback id
    /// </sumary>
    [Column("FEEDBACK_ID")]
    public int FeedbackId { get; set; }

    ///<summary>
    /// extension status (to review,denied,accepted)
    /// </summary>
    [Column("STATUS")]
    public string Status { get; set; }

    ///<summary>
    /// reason for extension
    /// </summary>
    [Column("REASON")]
    public string Reason { get; set; }

    ///<summary>
    /// extension length
    /// </summary>
    [Column("LENGTH")]
    public int Length { get; set; }
}
