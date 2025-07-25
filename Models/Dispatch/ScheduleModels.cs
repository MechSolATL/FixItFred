// FixItFred Patch Log — Sprint 30A Cleanup Patch
// [2025-07-25T00:00:00Z] — Removed duplicate models to resolve EF conflicts. Use MVP_Core.Data.Models only.
using System;
using System.Collections.Generic;

namespace MVP_Core.Models.Dispatch
{
    // FixItFred: Removed duplicate model (EF conflicts with Data.Models)
    // public class ScheduleQueue { ... }
    // public class ScheduleHistory { ... }
    // public class NotificationsSent { ... }
}
