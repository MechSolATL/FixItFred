// © 1997–2025 Virtual Concepts LLC, All Rights Reserved.
// Created & designed by Virtual Concepts LLC for Mechanical Solutions Atlanta.
// Platform: Service-Atlanta.com (MVP-Core vOmegaFinal)
// This software and all associated components are the exclusive intellectual property of Service Atlanta.
// No part of this system may be copied, distributed, resold, lent, or disclosed to any unauthorized party.
// Use is strictly limited to verified users who have completed Service Atlanta's full verification process.
// Unauthorized use without written authorization is enforceable by law.

using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Services
{
    /// <summary>
    /// TelemetryTraceService - Logs every login, overlay, CLI action for Sprint123_15-30
    /// Enforces GitHub push blocks via trace + GitGuard CI
    /// Persona drift detection via Lyra replay injection
    /// </summary>
    public class TelemetryTraceService
    {
        private readonly ILogger<TelemetryTraceService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _logPath;

        public TelemetryTraceService(ILogger<TelemetryTraceService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            
            // Ensure logs directory exists
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }

        /// <summary>
        /// Log user login events for security and audit purposes
        /// </summary>
        public async Task LogLoginAsync(string userId, string userName, string ipAddress, bool isSuccessful)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                EventType = "LOGIN",
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                IsSuccessful = isSuccessful,
                UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString(),
                SessionId = _httpContextAccessor.HttpContext?.Session.Id
            };

            await WriteTraceLogAsync("LoginTrace", logEntry);
            
            if (!isSuccessful)
            {
                _logger.LogWarning("Failed login attempt for user {UserName} from IP {IpAddress}", userName, ipAddress);
            }
        }

        /// <summary>
        /// Log CLI actions for FixItFred and Nova operations
        /// </summary>
        public async Task LogCliActionAsync(string action, string command, string userId, Dictionary<string, object>? parameters = null)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                EventType = "CLI_ACTION",
                Action = action,
                Command = command,
                UserId = userId,
                Parameters = parameters ?? new Dictionary<string, object>(),
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                SessionId = _httpContextAccessor.HttpContext?.Session.Id
            };

            await WriteTraceLogAsync("CliTrace", logEntry);
            await UpdateReplayHeatmapAsync(action, command);
        }

        /// <summary>
        /// Log Lyra overlay operations and persona interactions
        /// </summary>
        public async Task LogLyraOverlayAsync(string overlayType, string personaId, double empathyScore, string narrationContent)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                EventType = "LYRA_OVERLAY",
                OverlayType = overlayType,
                PersonaId = personaId,
                EmpathyScore = empathyScore,
                NarrationContent = narrationContent,
                SessionId = _httpContextAccessor.HttpContext?.Session.Id,
                UserId = _httpContextAccessor.HttpContext?.User?.Identity?.Name
            };

            await WriteTraceLogAsync("LyraTrace", logEntry);
            
            // Persona drift detection
            if (empathyScore < 0.85)
            {
                await LogPersonaDriftAsync(personaId, empathyScore);
            }
        }

        /// <summary>
        /// Log persona drift events when empathy threshold is not met
        /// </summary>
        public async Task LogPersonaDriftAsync(string personaId, double empathyScore)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                EventType = "PERSONA_DRIFT",
                PersonaId = personaId,
                EmpathyScore = empathyScore,
                Threshold = 0.85,
                AlertLevel = empathyScore < 0.5 ? "CRITICAL" : "WARNING",
                SessionId = _httpContextAccessor.HttpContext?.Session.Id
            };

            await WriteTraceLogAsync("PersonaDrift", logEntry);
            _logger.LogWarning("Persona drift detected for {PersonaId} with empathy score {EmpathyScore}", personaId, empathyScore);
        }

        /// <summary>
        /// Log GitHub operations for GitGuard CI enforcement
        /// </summary>
        public async Task LogGitHubOperationAsync(string operation, string repository, string branch, string commitHash, bool isBlocked = false)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                EventType = "GITHUB_OPERATION",
                Operation = operation,
                Repository = repository,
                Branch = branch,
                CommitHash = commitHash,
                IsBlocked = isBlocked,
                UserId = _httpContextAccessor.HttpContext?.User?.Identity?.Name,
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            };

            await WriteTraceLogAsync("GitHubTrace", logEntry);
            
            if (isBlocked)
            {
                _logger.LogWarning("GitHub operation {Operation} blocked for repository {Repository}", operation, repository);
            }
        }

        /// <summary>
        /// Write trace log entry to file
        /// </summary>
        private async Task WriteTraceLogAsync(string logType, object logEntry)
        {
            try
            {
                var logFileName = $"{logType}_{DateTime.UtcNow:yyyyMMdd}.log";
                var logFilePath = Path.Combine(_logPath, logFileName);
                
                var jsonEntry = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                
                await File.AppendAllTextAsync(logFilePath, jsonEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write trace log for {LogType}", logType);
            }
        }

        /// <summary>
        /// Update CLI simulation logs to ReplayTestHeatmap.md
        /// </summary>
        private async Task UpdateReplayHeatmapAsync(string action, string command)
        {
            try
            {
                var heatmapPath = Path.Combine(_logPath, "ReplayTestHeatmap.md");
                var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
                
                var heatmapEntry = $"## CLI Action: {action}\n" +
                                 $"**Command**: `{command}`\n" +
                                 $"**Timestamp**: {timestamp}\n" +
                                 $"**Session**: {_httpContextAccessor.HttpContext?.Session.Id}\n\n";

                await File.AppendAllTextAsync(heatmapPath, heatmapEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update replay heatmap");
            }
        }

        /// <summary>
        /// Get trace logs for CommandCenter dashboard
        /// </summary>
        public async Task<List<object>> GetTraceLogsAsync(string logType, DateTime? since = null)
        {
            var logs = new List<object>();
            
            try
            {
                var logFiles = Directory.GetFiles(_logPath, $"{logType}_*.log")
                    .OrderByDescending(f => f);

                foreach (var logFile in logFiles)
                {
                    var lines = await File.ReadAllLinesAsync(logFile);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            try
                            {
                                var logEntry = JsonSerializer.Deserialize<object>(line);
                                logs.Add(logEntry);
                            }
                            catch
                            {
                                // Skip malformed entries
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read trace logs for {LogType}", logType);
            }

            return logs;
        }
    }
}