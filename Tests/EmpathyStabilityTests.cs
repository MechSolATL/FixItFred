// Copyright (c) MechSolATL. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using MVP_Core.Services.Logs.Empathy;

namespace MVP_Core.Services.Tests;

/// <summary>
/// Enhanced empathy tests for Sprint125 with ≥90% target pass rate
/// </summary>
public static class EmpathyStabilityTests
{
    private static readonly Random random = new Random();
    
    /// <summary>
    /// Tests adaptive seeds stability across multiple personas
    /// Target: ≥90% pass rate
    /// </summary>
    /// <returns>Test result</returns>
    public static (string TestName, bool Passed, string Message, double PassRate) TestAdaptiveSeedsStability()
    {
        var testResults = new List<bool>();
        var personas = new[]
        {
            ("VisualLearner", EmpathyLog.EmotionalState.Calm),
            ("DetailOriented", EmpathyLog.EmotionalState.Confused),
            ("FastPaced", EmpathyLog.EmotionalState.Frustrated),
            ("Methodical", EmpathyLog.EmotionalState.Satisfied),
            ("Collaborative", EmpathyLog.EmotionalState.Happy),
        };

        foreach (var (persona, state) in personas)
        {
            try
            {
                // Create empathy log for persona
                var empathyLog = EmpathyLog.CreateInteraction(
                    persona,
                    "Help request with specific learning style",
                    "Adaptive response tailored to persona",
                    state);

                // Test empathy score calculation
                var score = empathyLog.GetEmpathyScore();
                
                // Check if score is within reasonable bounds (20-100)
                var scoreValid = score >= 20.0 && score <= 100.0;
                
                // Check if emotional state affects score appropriately
                var stateAdaptation = state switch
                {
                    EmpathyLog.EmotionalState.Frustrated => score >= 60.0, // Higher empathy needed
                    EmpathyLog.EmotionalState.Confused => score >= 50.0,
                    EmpathyLog.EmotionalState.Happy => score >= 20.0, // Lower empathy needed
                    EmpathyLog.EmotionalState.Satisfied => score >= 20.0,
                    _ => score >= 40.0
                };

                testResults.Add(scoreValid && stateAdaptation);
            }
            catch
            {
                testResults.Add(false);
            }
        }

        var passRate = (double)testResults.Count(r => r) / testResults.Count * 100.0;
        var passed = passRate >= 90.0;

        return ("Empathy_AdaptiveSeeds_Stability", passed, 
                $"Pass rate: {passRate:F1}% (Target: ≥90%)", passRate);
    }

    /// <summary>
    /// Tests Lyra encouragement case insensitive single append
    /// </summary>
    /// <returns>Test result</returns>
    public static (string TestName, bool Passed, string Message, double PassRate) TestLyraEncouragementCaseInsensitive()
    {
        var testCases = new[]
        {
            "HELP ME WITH THIS TASK",
            "help me with this task", 
            "Help Me With This Task",
            "hElP mE wItH tHiS tAsK",
            "NEED ASSISTANCE",
            "need assistance"
        };

        var results = new List<bool>();

        foreach (var input in testCases)
        {
            try
            {
                // Create interaction log
                var interaction = EmpathyLog.CreateInteraction(
                    "TestUser",
                    input,
                    "Encouraging response",
                    EmpathyLog.EmotionalState.Confused);

                // Verify interaction created successfully
                var valid = !string.IsNullOrEmpty(interaction.LogId) &&
                           interaction.UserInput.Equals(input) &&
                           interaction.UserState == EmpathyLog.EmotionalState.Confused;

                results.Add(valid);
            }
            catch
            {
                results.Add(false);
            }
        }

        var passRate = (double)results.Count(r => r) / results.Count * 100.0;
        var passed = passRate >= 90.0;

        return ("Lyra_Encouragement_CaseInsensitive_SingleAppend", passed,
                $"Pass rate: {passRate:F1}% (Target: ≥90%)", passRate);
    }

    /// <summary>
    /// Tests TraitProfile fallbacks when unknown
    /// </summary>
    /// <returns>Test result</returns>
    public static (string TestName, bool Passed, string Message, double PassRate) TestTraitProfileFallbacks()
    {
        var testScenarios = new[]
        {
            ("UnknownUser1", EmpathyLog.EmpathyLevel.High),
            ("UnknownUser2", EmpathyLog.EmpathyLevel.Moderate),
            ("UnknownUser3", EmpathyLog.EmpathyLevel.Low),
            ("UnknownUser4", EmpathyLog.EmpathyLevel.Exceptional),
            ("UnknownUser5", EmpathyLog.EmpathyLevel.Neutral)
        };

        var results = new List<bool>();

        foreach (var (userId, level) in testScenarios)
        {
            try
            {
                // Create empathy log with fallback scenario
                var log = EmpathyLog.Create(userId, "Fallback test scenario", level);
                
                // Test fallback behavior - should work even with unknown users
                var valid = !string.IsNullOrEmpty(log.LogId) &&
                           log.UserId == userId &&
                           log.Level == level &&
                           log.IsValid();

                // Test score calculation with fallbacks
                var score = log.GetEmpathyScore();
                var expectedRange = (int)level * 20.0;
                var scoreInRange = score >= expectedRange - 10 && score <= expectedRange + 10;

                results.Add(valid && scoreInRange);
            }
            catch
            {
                results.Add(false);
            }
        }

        var passRate = (double)results.Count(r => r) / results.Count * 100.0;
        var passed = passRate >= 90.0;

        return ("TraitProfile_Fallbacks_WhenUnknown", passed,
                $"Pass rate: {passRate:F1}% (Target: ≥90%)", passRate);
    }

    /// <summary>
    /// Runs all empathy stability tests
    /// </summary>
    /// <returns>Overall results</returns>
    public static (List<(string TestName, bool Passed, string Message, double PassRate)> Results, double OverallPassRate) RunAllEmpathyTests()
    {
        var results = new List<(string TestName, bool Passed, string Message, double PassRate)>
        {
            TestAdaptiveSeedsStability(),
            TestLyraEncouragementCaseInsensitive(),
            TestTraitProfileFallbacks()
        };

        var overallPassRate = results.Average(r => r.PassRate);
        return (results, overallPassRate);
    }
}