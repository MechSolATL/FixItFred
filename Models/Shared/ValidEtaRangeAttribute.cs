// FixItFred Patch Log — Sprint 28: Validation Engine
// [2024-07-25T00:30:00Z] — Custom validation attribute for ETA range.
using System;
using System.ComponentModel.DataAnnotations;
namespace MVP_Core.Models.Shared
{
    public class ValidEtaRangeAttribute : ValidationAttribute
    {
        public int MinMinutes { get; }
        public int MaxMinutes { get; }
        public ValidEtaRangeAttribute(int minMinutes, int maxMinutes)
        {
            MinMinutes = minMinutes;
            MaxMinutes = maxMinutes;
            ErrorMessage = $"ETA must be between {MinMinutes} and {MaxMinutes} minutes from now.";
        }
        public override bool IsValid(object? value)
        {
            if (value is DateTime eta)
            {
                var delta = (eta - DateTime.UtcNow).TotalMinutes;
                return delta >= MinMinutes && delta <= MaxMinutes;
            }
            return false;
        }
    }
}
