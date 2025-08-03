using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.IO;
using System.Threading.Tasks;

namespace Pages.Revitalize.Docs
{
    [Authorize(Roles = "Manager")]
    public class ManagerHandbookModel : PageModel
    {
        private readonly UserPerformanceLevelEngine _performanceEngine;

        public ManagerHandbookModel(UserPerformanceLevelEngine performanceEngine)
        {
            _performanceEngine = performanceEngine;
        }

        public string MarkdownContent { get; private set; } = string.Empty;
        public PerformanceLevelStatistics PerformanceStats { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Load and convert markdown content
                await LoadMarkdownContentAsync();
                
                // Load performance statistics
                PerformanceStats = await _performanceEngine.GetOverallPerformanceStatisticsAsync();

                return Page();
            }
            catch (FileNotFoundException)
            {
                MarkdownContent = "<p class='text-danger'><i class='fas fa-exclamation-triangle'></i> Manager Handbook content not found. Please contact your system administrator.</p>";
                return Page();
            }
            catch (Exception ex)
            {
                MarkdownContent = $"<p class='text-danger'><i class='fas fa-exclamation-triangle'></i> Error loading handbook: {ex.Message}</p>";
                return Page();
            }
        }

        private async Task LoadMarkdownContentAsync()
        {
            var contentPath = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "Handbooks", "Manager_Handbook.md");
            
            if (System.IO.File.Exists(contentPath))
            {
                var markdownText = await System.IO.File.ReadAllTextAsync(contentPath);
                MarkdownContent = ConvertMarkdownToHtml(markdownText);
            }
            else
            {
                throw new FileNotFoundException($"Manager handbook not found at: {contentPath}");
            }
        }

        private string ConvertMarkdownToHtml(string markdown)
        {
            // Simple markdown to HTML conversion
            // In a production environment, you'd use a proper markdown parser like Markdig
            
            // Process line by line for better control
            var lines = markdown.Split('\n');
            var result = new List<string>();
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    result.Add("");
                    continue;
                }
                
                // Headers
                if (trimmedLine.StartsWith("#### "))
                {
                    result.Add($"<h4>{trimmedLine.Substring(5)}</h4>");
                }
                else if (trimmedLine.StartsWith("### "))
                {
                    result.Add($"<h3>{trimmedLine.Substring(4)}</h3>");
                }
                else if (trimmedLine.StartsWith("## "))
                {
                    result.Add($"<h2>{trimmedLine.Substring(3)}</h2>");
                }
                else if (trimmedLine.StartsWith("# "))
                {
                    result.Add($"<h1>{trimmedLine.Substring(2)}</h1>");
                }
                // List items
                else if (trimmedLine.StartsWith("- "))
                {
                    result.Add($"<li>{ProcessInlineFormatting(trimmedLine.Substring(2))}</li>");
                }
                else if (trimmedLine.StartsWith("1. ") || trimmedLine.StartsWith("2. ") || trimmedLine.StartsWith("3. "))
                {
                    result.Add($"<li>{ProcessInlineFormatting(trimmedLine.Substring(3))}</li>");
                }
                // Horizontal rule
                else if (trimmedLine.StartsWith("---"))
                {
                    result.Add("<hr>");
                }
                // Regular paragraph
                else
                {
                    result.Add($"<p>{ProcessInlineFormatting(trimmedLine)}</p>");
                }
            }
            
            // Process lists
            var finalResult = new List<string>();
            bool inList = false;
            
            foreach (var line in result)
            {
                if (line.StartsWith("<li>"))
                {
                    if (!inList)
                    {
                        finalResult.Add("<ul>");
                        inList = true;
                    }
                    finalResult.Add(line);
                }
                else
                {
                    if (inList)
                    {
                        finalResult.Add("</ul>");
                        inList = false;
                    }
                    if (!string.IsNullOrEmpty(line))
                    {
                        finalResult.Add(line);
                    }
                }
            }
            
            if (inList)
            {
                finalResult.Add("</ul>");
            }
            
            return string.Join("\n", finalResult);
        }
        
        private string ProcessInlineFormatting(string text)
        {
            // Process bold text
            while (text.Contains("**"))
            {
                var firstIndex = text.IndexOf("**");
                var secondIndex = text.IndexOf("**", firstIndex + 2);
                if (secondIndex > firstIndex)
                {
                    var beforeFirst = text.Substring(0, firstIndex);
                    var content = text.Substring(firstIndex + 2, secondIndex - firstIndex - 2);
                    var afterSecond = text.Substring(secondIndex + 2);
                    text = beforeFirst + "<strong>" + content + "</strong>" + afterSecond;
                }
                else
                {
                    break;
                }
            }
            
            // Process code
            while (text.Contains("`"))
            {
                var firstIndex = text.IndexOf("`");
                var secondIndex = text.IndexOf("`", firstIndex + 1);
                if (secondIndex > firstIndex)
                {
                    var beforeFirst = text.Substring(0, firstIndex);
                    var content = text.Substring(firstIndex + 1, secondIndex - firstIndex - 1);
                    var afterSecond = text.Substring(secondIndex + 1);
                    text = beforeFirst + "<code>" + content + "</code>" + afterSecond;
                }
                else
                {
                    break;
                }
            }
            
            return text;
        }
    }
}