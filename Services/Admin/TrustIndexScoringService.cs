using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Models;

namespace Services.Admin
{
    public class TrustIndexScoringService
    {
        private readonly IMemoryCache _cache;
        public TrustIndexScoringService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public int ComputeTrustIndex(List<ViolationInsightModel> patterns)
        {
            if (patterns == null || patterns.Count == 0)
                return 100;
            double penalty = 0;
            foreach (var p in patterns)
            {
                // Example: weight by severity
                penalty += p.ConfidenceScore * PatternPenalty(p.PatternType);
            }
            int score = (int)Math.Round(100 - penalty);
            if (score < 0) score = 0;
            return score;
        }

        private double PatternPenalty(ViolationPatternType type)
        {
            return type switch
            {
                ViolationPatternType.LateStartStreak => 20,
                ViolationPatternType.MediaGaps => 15,
                ViolationPatternType.FakeLocationLoop => 30,
                ViolationPatternType.FrequentBackdating => 20,
                ViolationPatternType.GpsMismatch => 15,
                ViolationPatternType.ConsecutiveMissingUploads => 10,
                _ => 10
            };
        }

        public void CacheTrustIndex(int technicianId, int trustIndex)
        {
            _cache.Set($"TrustIndex_{technicianId}", trustIndex, TimeSpan.FromHours(24));
        }

        public int? GetCachedTrustIndex(int technicianId)
        {
            if (_cache.TryGetValue($"TrustIndex_{technicianId}", out int score))
                return score;
            return null;
        }
    }
}
