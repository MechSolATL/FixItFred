// Sprint 85.7 — Admin Log Hardening & Encryption
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace Helpers
{
    // Sprint 85.7 — Admin Log Hardening & Encryption
    public static class SensitiveNoteEncryptor
    {
        private static IDataProtector? _protector;
        public static void Configure(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("SensitiveNoteEncryptor");
        }
        public static string Encrypt(string plainText)
        {
            if (_protector == null) throw new InvalidOperationException("Protector not configured");
            return _protector.Protect(plainText);
        }
        public static string Decrypt(string cipherText)
        {
            if (_protector == null) throw new InvalidOperationException("Protector not configured");
            return _protector.Unprotect(cipherText);
        }
    }
}
