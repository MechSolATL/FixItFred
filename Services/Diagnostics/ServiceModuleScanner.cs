// Sprint91_27 - Service module scanner for DI registration diagnostics
using System.Reflection;

namespace MVP_Core.Services.Diagnostics
{
    public interface IServiceModuleScanner
    {
        /// <summary>
        /// Scan assemblies for unregistered DI services
        /// </summary>
        Task<List<string>> ScanForUnregisteredServices();

        /// <summary>
        /// Get all registered service types
        /// </summary>
        List<Type> GetRegisteredServices();

        /// <summary>
        /// Log scan results to FixItFred logs
        /// </summary>
        Task LogScanResults(List<string> unregisteredServices);
    }

    public class ServiceModuleScanner : IServiceModuleScanner
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServiceModuleScanner> _logger;
        private readonly string _logPath;

        public ServiceModuleScanner(IServiceProvider serviceProvider, ILogger<ServiceModuleScanner> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _logPath = Path.Combine("Logs", "FixItFred");
            
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }

        public async Task<List<string>> ScanForUnregisteredServices()
        {
            _logger.LogInformation("Starting service module scan for unregistered DI services");

            var unregisteredServices = new List<string>();
            var registeredTypes = GetRegisteredServices();
            
            // Get all types in Services namespace
            var serviceTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace?.StartsWith("MVP_Core.Services") == true || 
                           t.Namespace?.StartsWith("Services") == true)
                .Where(t => t.IsClass && !t.IsAbstract)
                .ToList();

            foreach (var serviceType in serviceTypes)
            {
                // Check if service implements an interface
                var interfaces = serviceType.GetInterfaces()
                    .Where(i => i.Namespace?.StartsWith("MVP_Core") == true || 
                               i.Namespace?.StartsWith("Services") == true)
                    .ToList();

                foreach (var interfaceType in interfaces)
                {
                    if (!registeredTypes.Contains(interfaceType) && !registeredTypes.Contains(serviceType))
                    {
                        unregisteredServices.Add($"{interfaceType.Name} -> {serviceType.Name}");
                    }
                }

                // Also check standalone services
                if (!interfaces.Any() && !registeredTypes.Contains(serviceType))
                {
                    unregisteredServices.Add($"Standalone service: {serviceType.Name}");
                }
            }

            await LogScanResults(unregisteredServices);
            return unregisteredServices;
        }

        public List<Type> GetRegisteredServices()
        {
            // Sprint91_27 - Extract registered service types from DI container
            var registeredTypes = new List<Type>();
            
            try
            {
                // Use service provider to get registered services
                // This is a simplified approach for scanning available services
                var serviceType = typeof(IServiceProvider);
                registeredTypes.Add(serviceType);
                
                // Add known service types that are commonly registered
                // This could be expanded with more sophisticated reflection
                registeredTypes.Add(typeof(ILogger<>));
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not extract registered services: {ex.Message}");
            }

            return registeredTypes;
        }

        public async Task LogScanResults(List<string> unregisteredServices)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var logFileName = $"ModuleScan_{timestamp}.log";
            var logFilePath = Path.Combine(_logPath, logFileName);

            var logContent = $"FixItFred Service Module Scan Report\n";
            logContent += $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n";
            logContent += $"Sprint: 91_27 - Operation System Fusion\n\n";

            if (unregisteredServices.Any())
            {
                logContent += $"UNREGISTERED SERVICES DETECTED ({unregisteredServices.Count}):\n";
                logContent += string.Join("\n", unregisteredServices.Select(s => $"  - {s}"));
                logContent += "\n\n";
                logContent += "RECOMMENDATION: Review Program.cs for missing service registrations\n";
            }
            else
            {
                logContent += "✅ ALL SERVICES PROPERLY REGISTERED\n";
            }

            logContent += $"\nScan completed at {DateTime.UtcNow}\n";

            await File.WriteAllTextAsync(logFilePath, logContent);
            _logger.LogInformation($"Service scan results logged to {logFilePath}");
        }
    }
}