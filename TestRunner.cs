// Copyright (c) MechSolATL. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MVP_Core.Services.Tests;

namespace MVP_Core.Services;

/// <summary>
/// Enhanced test runner for Sprint125 validation
/// </summary>
public class TestRunner
{
    /// <summary>
    /// Runs all Sprint125 tests including empathy and SEO compliance
    /// </summary>
    public static void RunTests()
    {
        Console.WriteLine("🚀 Starting Sprint125 Test Suite...\n");
        
        try
        {
            // Run original integration tests
            Console.WriteLine("=== Sprint122 Integration Tests ===");
            Sprint122IntegrationTests.RunAllTests();
            
            Console.WriteLine("\n=== Sprint125 Empathy Stability Tests ===");
            RunEmpathyStabilityTests();
            
            Console.WriteLine("\n=== Sprint125 SEO Compliance Tests ===");
            RunSeoComplianceTests();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test suite execution failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine("\n📝 Sprint125 test execution completed.");
    }

    private static void RunEmpathyStabilityTests()
    {
        var (results, overallPassRate) = EmpathyStabilityTests.RunAllEmpathyTests();
        
        foreach (var result in results)
        {
            var status = result.Passed ? "✅ PASS" : "❌ FAIL";
            Console.WriteLine($"{status}: {result.TestName} - {result.Message}");
        }
        
        Console.WriteLine($"\n📊 Overall Empathy Pass Rate: {overallPassRate:F1}%");
        
        if (overallPassRate >= 90.0)
        {
            Console.WriteLine("🎉 EMPATHY TARGET ACHIEVED - ≥90% pass rate!");
        }
        else
        {
            Console.WriteLine($"⚠️  Empathy target not met. Need {90.0 - overallPassRate:F1}% improvement.");
        }
    }

    private static void RunSeoComplianceTests()
    {
        var results = SeoComplianceTests.RunAllSeoTests();
        
        var passedTests = 0;
        foreach (var result in results)
        {
            var status = result.Passed ? "✅ PASS" : "❌ FAIL";
            Console.WriteLine($"{status}: {result.TestName} - {result.Message}");
            if (result.Passed) passedTests++;
        }
        
        var complianceRate = (double)passedTests / results.Length * 100.0;
        Console.WriteLine($"\n📊 SEO Compliance Rate: {complianceRate:F1}%");
        
        if (complianceRate >= 100.0)
        {
            Console.WriteLine("🎉 SEO COMPLIANCE ACHIEVED - All tests green!");
        }
        else
        {
            Console.WriteLine($"⚠️  SEO compliance gaps found. {results.Length - passedTests} test(s) failed.");
        }
    }
}