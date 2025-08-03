namespace MVP_Core.Services.Tools.FixItFred.Diagnostics;

/// <summary>
/// FixItFred Telemetry class for Sprint122_CertumDNSBypass
/// Handles telemetry collection, processing, and privacy management
/// </summary>
public class FixItFredTelemetry
{
    private readonly Dictionary<string, TelemetryMetric> _metrics;
    private readonly Queue<TelemetryEvent> _eventQueue;
    private readonly DateTime _initializationTime;
    private bool _isEnabled;
    private bool _privacyModeEnabled;
    
    public bool IsEnabled => _isEnabled;
    public bool PrivacyModeEnabled => _privacyModeEnabled;
    public int EventCount => _eventQueue.Count;
    public int MetricCount => _metrics.Count;
    
    public FixItFredTelemetry(bool enabledByDefault = true, bool privacyMode = false)
    {
        _metrics = new Dictionary<string, TelemetryMetric>();
        _eventQueue = new Queue<TelemetryEvent>();
        _initializationTime = DateTime.UtcNow;
        _isEnabled = enabledByDefault;
        _privacyModeEnabled = privacyMode;
        
        InitializeBasicMetrics();
        LogInitialization();
    }
    
    /// <summary>
    /// Telemetry metric representation
    /// </summary>
    public class TelemetryMetric
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public object Value { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public long UpdateCount { get; set; } = 0;
        public Dictionary<string, object> Tags { get; set; } = new();
    }
    
    /// <summary>
    /// Telemetry event representation
    /// </summary>
    public class TelemetryEvent
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public string EventName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Properties { get; set; } = new();
        public TelemetrySeverity Severity { get; set; } = TelemetrySeverity.Info;
    }
    
    public enum TelemetrySeverity
    {
        Verbose = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Critical = 5
    }
    
    /// <summary>
    /// Enables telemetry collection
    /// </summary>
    public void Enable()
    {
        _isEnabled = true;
        TrackEvent("TelemetryEnabled", "System", TelemetrySeverity.Info);
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] TELEMETRY_ENABLED: FixItFred telemetry collection activated");
    }
    
    /// <summary>
    /// Disables telemetry collection for privacy
    /// </summary>
    public void Disable()
    {
        _isEnabled = false;
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] TELEMETRY_DISABLED: FixItFred telemetry collection deactivated for privacy");
    }
    
    /// <summary>
    /// Enables privacy mode - minimal data collection
    /// </summary>
    public void EnablePrivacyMode()
    {
        _privacyModeEnabled = true;
        TrackEvent("PrivacyModeEnabled", "Privacy", TelemetrySeverity.Info);
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] PRIVACY_MODE_ENABLED: Minimal telemetry collection only");
    }
    
    /// <summary>
    /// Disables privacy mode - full telemetry collection
    /// </summary>
    public void DisablePrivacyMode()
    {
        _privacyModeEnabled = false;
        TrackEvent("PrivacyModeDisabled", "Privacy", TelemetrySeverity.Info);
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] PRIVACY_MODE_DISABLED: Full telemetry collection enabled");
    }
    
    /// <summary>
    /// Tracks a telemetry event
    /// </summary>
    public void TrackEvent(string eventName, string category = "General", TelemetrySeverity severity = TelemetrySeverity.Info, Dictionary<string, object>? properties = null)
    {
        if (!_isEnabled)
            return;
            
        // In privacy mode, only track essential events
        if (_privacyModeEnabled && severity < TelemetrySeverity.Warning)
            return;
            
        var telemetryEvent = new TelemetryEvent
        {
            EventName = eventName,
            Category = category,
            Severity = severity,
            Properties = properties ?? new Dictionary<string, object>()
        };
        
        // Add privacy-safe system properties
        if (!_privacyModeEnabled)
        {
            telemetryEvent.Properties["sprint"] = "Sprint122_CertumDNSBypass";
            telemetryEvent.Properties["component"] = "FixItFred";
        }
        
        _eventQueue.Enqueue(telemetryEvent);
        
        // Keep queue size manageable
        while (_eventQueue.Count > 1000)
        {
            _eventQueue.Dequeue();
        }
    }
    
    /// <summary>
    /// Updates a telemetry metric
    /// </summary>
    public void UpdateMetric(string name, object value, string category = "Performance", Dictionary<string, object>? tags = null)
    {
        if (!_isEnabled)
            return;
            
        if (_metrics.TryGetValue(name, out var existingMetric))
        {
            existingMetric.Value = value;
            existingMetric.LastUpdated = DateTime.UtcNow;
            existingMetric.UpdateCount++;
            
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    existingMetric.Tags[tag.Key] = tag.Value;
                }
            }
        }
        else
        {
            _metrics[name] = new TelemetryMetric
            {
                Name = name,
                Category = category,
                Value = value,
                UpdateCount = 1,
                Tags = tags ?? new Dictionary<string, object>()
            };
        }
    }
    
    /// <summary>
    /// Increments a counter metric
    /// </summary>
    public void IncrementCounter(string name, int increment = 1, string category = "Counters")
    {
        if (!_isEnabled)
            return;
            
        if (_metrics.TryGetValue(name, out var metric) && metric.Value is int currentValue)
        {
            UpdateMetric(name, currentValue + increment, category);
        }
        else
        {
            UpdateMetric(name, increment, category);
        }
    }
    
    /// <summary>
    /// Records execution time for operations
    /// </summary>
    public void RecordExecutionTime(string operationName, TimeSpan duration)
    {
        if (!_isEnabled)
            return;
            
        UpdateMetric($"{operationName}_duration_ms", duration.TotalMilliseconds, "Performance");
        IncrementCounter($"{operationName}_executions", 1, "Counters");
    }
    
    /// <summary>
    /// Starts tracking an operation duration
    /// </summary>
    public TelemetryTimer StartTimer(string operationName)
    {
        return new TelemetryTimer(this, operationName);
    }
    
    /// <summary>
    /// Gets telemetry summary
    /// </summary>
    public string GetTelemetrySummary()
    {
        var uptime = DateTime.UtcNow - _initializationTime;
        var status = _isEnabled ? (_privacyModeEnabled ? "Privacy Mode" : "Full Mode") : "Disabled";
        
        return $"FixItFredTelemetry: {status}, Events: {_eventQueue.Count}, Metrics: {_metrics.Count}, Uptime: {uptime:hh\\:mm\\:ss}";
    }
    
    /// <summary>
    /// Exports telemetry data for analysis
    /// </summary>
    public object ExportTelemetryData(bool includeSensitiveData = false)
    {
        if (!_isEnabled)
            return new { Status = "Disabled" };
            
        var events = _eventQueue.ToList();
        
        // Filter sensitive data in privacy mode
        if (_privacyModeEnabled && !includeSensitiveData)
        {
            events = events.Where(e => e.Severity >= TelemetrySeverity.Warning).ToList();
        }
        
        return new
        {
            Status = GetTelemetrySummary(),
            InitializationTime = _initializationTime,
            IsEnabled = _isEnabled,
            PrivacyMode = _privacyModeEnabled,
            Events = events,
            Metrics = _metrics.Values.ToList(),
            ExportTimestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Clears all telemetry data for privacy
    /// </summary>
    public void ClearTelemetryData()
    {
        _eventQueue.Clear();
        _metrics.Clear();
        InitializeBasicMetrics();
        
        TrackEvent("TelemetryDataCleared", "Privacy", TelemetrySeverity.Info);
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] TELEMETRY_CLEARED: All telemetry data cleared for privacy");
    }
    
    private void InitializeBasicMetrics()
    {
        UpdateMetric("initialization_time", _initializationTime.ToString("yyyy-MM-dd HH:mm:ss"), "System");
        UpdateMetric("sprint_version", "Sprint122_CertumDNSBypass", "System");
        UpdateMetric("component_name", "FixItFred", "System");
    }
    
    private void LogInitialization()
    {
        var mode = _privacyModeEnabled ? "Privacy Mode" : "Full Mode";
        var status = _isEnabled ? "Enabled" : "Disabled";
        
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] TELEMETRY_INIT: FixItFredTelemetry initialized - {status} ({mode})");
        
        if (_isEnabled)
        {
            TrackEvent("TelemetryInitialized", "System", TelemetrySeverity.Info, new Dictionary<string, object>
            {
                ["privacy_mode"] = _privacyModeEnabled,
                ["sprint"] = "Sprint122_CertumDNSBypass"
            });
        }
    }
    
    /// <summary>
    /// Timer helper for measuring operation duration
    /// </summary>
    public class TelemetryTimer : IDisposable
    {
        private readonly FixItFredTelemetry _telemetry;
        private readonly string _operationName;
        private readonly DateTime _startTime;
        
        public TelemetryTimer(FixItFredTelemetry telemetry, string operationName)
        {
            _telemetry = telemetry;
            _operationName = operationName;
            _startTime = DateTime.UtcNow;
        }
        
        public void Dispose()
        {
            var duration = DateTime.UtcNow - _startTime;
            _telemetry.RecordExecutionTime(_operationName, duration);
        }
    }
}