namespace Interfaces;

/// <summary>
/// Interface for Lyra cognition empathy trigger resolution
/// </summary>
public interface ILyraCognition
{
    /// <summary>
    /// Resolves empathy prompt based on context
    /// </summary>
    /// <param name="context">The context for empathy resolution</param>
    /// <returns>Resolved empathy prompt</returns>
    Task<string> ResolvePromptAsync(string context);
}