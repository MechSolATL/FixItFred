namespace Services
{
    /// <summary>
    /// Interface for customer ticket analytics service.
    /// [FixItFredComment:Sprint1004 - DI registration verified] Created interface for proper DI registration
    /// </summary>
    public interface ICustomerTicketAnalyticsService
    {
        /// <summary>
        /// Get total ticket count for a customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>Total ticket count.</returns>
        int GetTicketCountForCustomer(int customerId);

        /// <summary>
        /// Get average response time (in minutes) for a customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>Average response time in minutes.</returns>
        double GetAverageResponseTime(int customerId);

        /// <summary>
        /// Get satisfaction rating trend (last 10 tickets, most recent first).
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>Array of satisfaction ratings.</returns>
        int[] GetSatisfactionRatingTrend(int customerId);
    }
}