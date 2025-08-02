using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Interfaces;
using System.Text.Json;

namespace Revitalize.Services;

/// <summary>
/// RevitalizeReplayCLI for empathy prompt streaming and persona trait annotation
/// Sprint121+122: CLI integration with cognitive seeds and persona traits
/// </summary>
public class RevitalizeReplayCLI
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RevitalizeReplayCLI> _logger;
    private readonly ILyraCognition _lyraCognition;

    public RevitalizeReplayCLI(
        IConfiguration configuration,
        ILogger<RevitalizeReplayCLI> logger,
        ILyraCognition lyraCognition)
    {
        _configuration = configuration;
        _logger = logger;
        _lyraCognition = lyraCognition;
    }

    /// <summary>
    /// Streams empathy prompts with persona trait annotations
    /// </summary>
    /// <param name="persona">Customer persona (AnxiousCustomer, FrustratedCustomer, TechnicallySavvy)</param>
    /// <param name="context">Context for empathy prompt</param>
    /// <returns>Annotated empathy response</returns>
    public async Task<string> StreamEmpathyPromptAsync(string persona, string context)
    {
        _logger.LogInformation("Streaming empathy prompt for persona: {Persona}, context: {Context}", persona, context);

        try
        {
            // Get base empathy response from Lyra
            var baseResponse = await _lyraCognition.ResolvePromptAsync(context);
            
            // Annotate with persona traits
            var annotatedResponse = AnnotateWithPersonaTraits(baseResponse, persona);
            
            _logger.LogDebug("Generated annotated response for {Persona}: {Response}", persona, annotatedResponse);
            
            return annotatedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error streaming empathy prompt for persona {Persona}", persona);
            return $"Persona: {persona} - Error generating empathy response";
        }
    }

    /// <summary>
    /// Accepts JSON input from RevitalizeCognitiveSeeds.json for testing scenarios
    /// </summary>
    /// <param name="cognitiveJsonPath">Path to cognitive seeds JSON file</param>
    /// <returns>List of processed empathy scenarios</returns>
    public async Task<List<EmpathyScenario>> ProcessCognitiveSeedsAsync(string cognitiveJsonPath)
    {
        _logger.LogInformation("Processing cognitive seeds from: {Path}", cognitiveJsonPath);

        try
        {
            if (!File.Exists(cognitiveJsonPath))
            {
                _logger.LogWarning("Cognitive seeds file not found: {Path}", cognitiveJsonPath);
                return new List<EmpathyScenario>();
            }

            var jsonContent = await File.ReadAllTextAsync(cognitiveJsonPath);
            var cognitiveData = JsonSerializer.Deserialize<CognitiveSeedsData>(jsonContent);

            if (cognitiveData?.Scenarios == null)
            {
                _logger.LogWarning("No scenarios found in cognitive seeds file");
                return new List<EmpathyScenario>();
            }

            var processedScenarios = new List<EmpathyScenario>();

            foreach (var scenario in cognitiveData.Scenarios)
            {
                var response = await StreamEmpathyPromptAsync(scenario.Persona, scenario.Context);
                
                processedScenarios.Add(new EmpathyScenario
                {
                    Persona = scenario.Persona,
                    Context = scenario.Context,
                    ExpectedResponse = scenario.ExpectedResponse,
                    ActualResponse = response,
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation("Processed {Count} cognitive scenarios", processedScenarios.Count);
            return processedScenarios;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing cognitive seeds from {Path}", cognitiveJsonPath);
            return new List<EmpathyScenario>();
        }
    }

    /// <summary>
    /// Annotates empathy response with persona-specific traits
    /// </summary>
    /// <param name="baseResponse">Base empathy response</param>
    /// <param name="persona">Customer persona</param>
    /// <returns>Annotated response with persona traits</returns>
    private string AnnotateWithPersonaTraits(string baseResponse, string persona)
    {
        var annotation = persona.ToLower() switch
        {
            "anxiouscustomer" => "[Persona: AnxiousCustomer - Requires extra reassurance and clear timelines]",
            "frustratedcustomer" => "[Persona: FrustratedCustomer - Needs acknowledgment of frustration and immediate action]",
            "technicallysavvy" => "[Persona: TechnicallySavvy - Appreciates detailed technical explanations]",
            _ => $"[Persona: {persona} - General empathy approach]"
        };

        return $"{annotation}\n{baseResponse}";
    }

    /// <summary>
    /// Gets debug replay information if enabled in configuration
    /// </summary>
    /// <returns>Debug replay information</returns>
    public string GetDebugReplayInfo()
    {
        var enableDebugReplay = _configuration.GetValue<bool>("Revitalize:EnableDebugReplay");
        
        if (!enableDebugReplay)
        {
            return "Debug replay is disabled";
        }

        return $"Debug Replay Enabled - Platform: {_configuration["Revitalize:PlatformName"]}, " +
               $"Version: {_configuration["Revitalize:Version"]}, " +
               $"Empathy Mode: {_configuration["Features:EmpathyMode"]}";
    }
}

/// <summary>
/// Data model for cognitive seeds JSON structure
/// </summary>
public class CognitiveSeedsData
{
    public List<CognitiveScenario> Scenarios { get; set; } = new();
}

/// <summary>
/// Individual cognitive scenario from JSON
/// </summary>
public class CognitiveScenario
{
    public string Persona { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string ExpectedResponse { get; set; } = string.Empty;
}

/// <summary>
/// Processed empathy scenario with actual response
/// </summary>
public class EmpathyScenario
{
    public string Persona { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string ExpectedResponse { get; set; } = string.Empty;
    public string ActualResponse { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}