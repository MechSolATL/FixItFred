using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class PermissionService
    {
        public bool CanAccess(AdminUser user, string moduleName)
        {
            if (user?.EnabledModules == null)
                return false;

            return user.EnabledModules.Contains(moduleName, StringComparer.OrdinalIgnoreCase);
        }

        // ?? Future: Return all available modules for UI rendering
        public static List<string> GetAllModules()
        {
            return new List<string>
            {
                "CommunityBoard",
                "Dispatcher",
                "Leaderboard",
                "BehaviorInsights",
                "FlagReviewQueue",
                "TechnicianStatusReport"
            };
        }
    }
}
