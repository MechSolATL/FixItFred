using System;
using Microsoft.Extensions.Logging;
using Data.Models;

namespace Services
{
    public class AuditLogger
    {
        private readonly ILogger<AuditLogger> _logger;
        private readonly AuditLogEncryptionService _encryptionService;
        public AuditLogger(ILogger<AuditLogger> logger, AuditLogEncryptionService encryptionService)
        {
            _logger = logger;
            _encryptionService = encryptionService;
        }

        public AuditLog CreateEncryptedLog(string action, int userId, string oldValue, string newValue, string ip, string securityLevel = "Standard")
        {
            var encryptedOld = _encryptionService.Encrypt(oldValue);
            var encryptedNew = _encryptionService.Encrypt(newValue);
            var encryptedIp = _encryptionService.Encrypt(ip);
            var hash = AuditLogEncryptionService.ComputeSHA256($"{userId}|{action}|{encryptedOld}|{encryptedNew}|{encryptedIp}|{securityLevel}");
            return new AuditLog
            {
                UserId = userId,
                ChangeType = action,
                OldValueEncrypted = encryptedOld,
                NewValueEncrypted = encryptedNew,
                IPAddressEncrypted = encryptedIp,
                IntegrityHash = hash,
                Timestamp = DateTime.UtcNow,
                // Add SecurityLevel if present in model
            };
        }

        public void DecryptLog(AuditLog log)
        {
            log.OldValue = _encryptionService.Decrypt(log.OldValueEncrypted);
            log.NewValue = _encryptionService.Decrypt(log.NewValueEncrypted);
            log.IPAddress = _encryptionService.Decrypt(log.IPAddressEncrypted);
        }
    }
}
