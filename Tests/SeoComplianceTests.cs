// Copyright (c) MechSolATL. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;

namespace MVP_Core.Services.Tests;

/// <summary>
/// SEO compliance tests for Sprint125 Razor pages
/// </summary>
public static class SeoComplianceTests
{
    /// <summary>
    /// Tests that Razor pages set Title, Layout, and SEO metadata
    /// </summary>
    /// <returns>Test result</returns>
    public static (string TestName, bool Passed, string Message) TestRazorSetsRequiredSeoElements()
    {
        try
        {
            var pagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Pages");
            if (!Directory.Exists(pagesDirectory))
            {
                return ("Razor_Sets_Title_Layout_Seo_For_PageSet", false, "Pages directory not found");
            }

            var razorFiles = Directory.GetFiles(pagesDirectory, "*.cshtml", SearchOption.AllDirectories)
                .Where(f => !Path.GetFileName(f).StartsWith("_")) // Exclude partials
                .ToArray();

            if (razorFiles.Length == 0)
            {
                return ("Razor_Sets_Title_Layout_Seo_For_PageSet", false, "No Razor pages found");
            }

            var compliantPages = 0;
            var totalPages = razorFiles.Length;

            foreach (var file in razorFiles)
            {
                var content = File.ReadAllText(file);
                
                // Check for ViewData["Title"]
                var hasTitle = content.Contains("ViewData[\"Title\"]");
                
                // Check for Layout specification
                var hasLayout = content.Contains("Layout") && 
                               (content.Contains("_Layout.cshtml") || content.Contains("Layout ="));

                // Check for corresponding .cs file with OnGetAsync
                var csFile = file + ".cs";
                var hasPageModel = File.Exists(csFile);
                var hasOnGetAsync = false;
                var loadsSeo = false;

                if (hasPageModel)
                {
                    var csContent = File.ReadAllText(csFile);
                    hasOnGetAsync = csContent.Contains("OnGetAsync");
                    loadsSeo = csContent.Contains("MetaDescription") || 
                              csContent.Contains("Keywords") || 
                              csContent.Contains("Robots");
                }

                if (hasTitle && hasLayout && (!hasPageModel || (hasOnGetAsync && loadsSeo)))
                {
                    compliantPages++;
                }
            }

            var complianceRate = (double)compliantPages / totalPages * 100.0;
            var passed = complianceRate >= 100.0; // Target: 100% compliance

            return ("Razor_Sets_Title_Layout_Seo_For_PageSet", passed,
                    $"Compliant pages: {compliantPages}/{totalPages} ({complianceRate:F1}%)");
        }
        catch (Exception ex)
        {
            return ("Razor_Sets_Title_Layout_Seo_For_PageSet", false, $"Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Tests that SEO robots meta tag is respected for index vs noindex
    /// </summary>
    /// <returns>Test result</returns>
    public static (string TestName, bool Passed, string Message) TestSeoRobotsRespected()
    {
        try
        {
            var seoHeadPath = Path.Combine(Directory.GetCurrentDirectory(), "Pages", "Shared", "_SEOHead.cshtml");
            
            if (!File.Exists(seoHeadPath))
            {
                return ("Seo_Robots_Respected_IndexVsNoindex", false, "_SEOHead.cshtml partial not found");
            }

            var content = File.ReadAllText(seoHeadPath);
            
            // Check for robots meta tag handling
            var hasRobotsHandling = content.Contains("robots") && content.Contains("ViewData[\"Robots\"]");
            
            // Check for default robots fallback
            var hasDefaultRobots = content.Contains("index,follow");
            
            // Check for Open Graph and Twitter Card support
            var hasOpenGraph = content.Contains("og:title") && content.Contains("og:description");
            var hasTwitterCard = content.Contains("twitter:card") && content.Contains("twitter:title");

            var passed = hasRobotsHandling && hasDefaultRobots && hasOpenGraph && hasTwitterCard;

            var message = passed 
                ? "SEO head properly handles robots directives and social media meta tags"
                : $"Missing: Robots={hasRobotsHandling}, Default={hasDefaultRobots}, OG={hasOpenGraph}, Twitter={hasTwitterCard}";

            return ("Seo_Robots_Respected_IndexVsNoindex", passed, message);
        }
        catch (Exception ex)
        {
            return ("Seo_Robots_Respected_IndexVsNoindex", false, $"Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Runs all SEO compliance tests
    /// </summary>
    /// <returns>Test results</returns>
    public static (string TestName, bool Passed, string Message)[] RunAllSeoTests()
    {
        return new[]
        {
            TestRazorSetsRequiredSeoElements(),
            TestSeoRobotsRespected()
        };
    }
}