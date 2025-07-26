using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Services
{
    public class SurveyService
    {
        private readonly ApplicationDbContext _db;
        private readonly FeedbackAnalyticsService _analyticsService;
        public SurveyService(ApplicationDbContext db)
        {
            _db = db;
            _analyticsService = new FeedbackAnalyticsService(db);
        }

        public List<FeedbackSurveyTemplate> GetSurveyByServiceType(string serviceType)
        {
            return _db.FeedbackSurveyTemplates
                .Where(q => q.ServiceType == serviceType)
                .OrderBy(q => q.SortOrder)
                .ToList();
        }

        public void SaveResponses(List<FeedbackResponse> responses)
        {
            foreach (var response in responses)
            {
                _db.FeedbackResponses.Add(response);
                // Sentiment/keyword analysis
                var sentiment = _analyticsService.CalculateSentiment(response.ResponseValue);
                var keywords = _analyticsService.ExtractKeywords(response.ResponseValue);
                // Optionally store sentiment/keywords in response or related analytics table
            }
            _db.SaveChanges();
        }

        public List<FeedbackResponse> GetResponsesForRequest(int serviceRequestId)
        {
            return _db.FeedbackResponses.Where(r => r.ServiceRequestId == serviceRequestId).ToList();
        }
    }
}
