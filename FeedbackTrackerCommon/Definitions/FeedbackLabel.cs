namespace Core.Definitions;

public enum FeedbackLabel
{
    /// <summary>
    /// Positive feedback
    /// </summary>
    Positive = 0,
    
    /// <summary>
    /// Feedback suggesting what can be improved.
    /// </summary>
    Improvement = 1,
    
    /// <summary>
    /// Question about module
    /// </summary>
    Question = 2,
    
    /// <summary>
    /// Mistake/Error that needs fixing
    /// </summary>
    Error = 3
}