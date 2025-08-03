// Sprint91_27 - Calendar sync service interface for Google and Outlook integration
using Data.Models;

namespace MVP_Core.Services
{
    public interface ICalendarSyncService
    {
        /// <summary>
        /// Sync service request with Google Calendar
        /// </summary>
        /// <param name="serviceRequest">Service request to sync</param>
        /// <returns>Task representing the async operation</returns>
        Task SyncGoogleEvent(ServiceRequest serviceRequest);

        /// <summary>
        /// Sync service request with Outlook Calendar
        /// </summary>
        /// <param name="serviceRequest">Service request to sync</param>
        /// <returns>Task representing the async operation</returns>
        Task SyncOutlookEvent(ServiceRequest serviceRequest);
    }
}