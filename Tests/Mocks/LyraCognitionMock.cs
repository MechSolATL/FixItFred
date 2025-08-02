using Interfaces;

namespace Tests.Mocks;

/// <summary>
/// Mock implementation of ILyraCognition for testing empathy trigger resolution logic
/// Sprint121: Tactical add-on for DI-enabled test framework
/// </summary>
public class LyraCognitionMock : ILyraCognition
{
    private readonly Dictionary<string, string> _mockResponses = new()
    {
        {"service_failure", "I'm sorry to hear that your service isn't working as expected."},
        {"billing_issue", "I understand your concern about the billing. Let me help resolve this."},
        {"scheduling_conflict", "I apologize for the scheduling confusion. Let's find a better time."},
        {"general_complaint", "Thank you for bringing this to our attention. We'll make it right."},
        {"urgent_request", "I recognize this is urgent. Let me prioritize this for you."}
    };

    /// <summary>
    /// Simulates empathy prompt resolution based on context
    /// </summary>
    /// <param name="context">The context for empathy resolution</param>
    /// <returns>Resolved empathy prompt</returns>
    public Task<string> ResolvePromptAsync(string context)
    {
        if (string.IsNullOrWhiteSpace(context))
        {
            return Task.FromResult("I'm here to help you.");
        }

        var lowerContext = context.ToLower();
        
        // Check for specific context matches
        foreach (var kvp in _mockResponses)
        {
            if (lowerContext.Contains(kvp.Key.Replace("_", " ")))
            {
                return Task.FromResult(kvp.Value);
            }
        }

        // Default empathetic response
        return Task.FromResult("Resolved empathy prompt: [Mock] - I understand and I'm here to help.");
    }

    /// <summary>
    /// Adds a custom mock response for testing specific scenarios
    /// </summary>
    /// <param name="context">Context keyword</param>
    /// <param name="response">Mock response</param>
    public void AddMockResponse(string context, string response)
    {
        _mockResponses[context] = response;
    }

    /// <summary>
    /// Clears all mock responses
    /// </summary>
    public void ClearMockResponses()
    {
        _mockResponses.Clear();
    }
}