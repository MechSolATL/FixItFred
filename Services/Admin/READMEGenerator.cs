using System;
using System.IO;
using MVP_Core.Data.Models;
using MVP_Core.Data;
using System.Text.Json;

namespace MVP_Core.Services.Admin
{
    // Sprint 83.4: README generator for onboarding data
    public static class READMEGenerator
    {
        public static string GenerateRawReadme(EmployeeOnboardingProfile profile)
        {
            var data = new
            {
                profile.UserId,
                profile.CompanyName,
                profile.ContactName,
                profile.Signature,
                profile.FilePaths,
                profile.IsVerified,
                profile.CreatedOn
            };
            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
