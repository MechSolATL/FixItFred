using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;

namespace MVP_Core.Services.Reports
{
    public class TechnicianReportService
    {
        public byte[] GenerateSingleReport(TechnicianProfileDto tech, byte[] chartImage, string? notes)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Technician Performance Report").FontSize(20).Bold();
                            col.Item().Text($"{tech.FullName}").FontSize(14);
                        });
                        var logoDescriptor = row.ConstantItem(80).Image("wwwroot/img/logo.png");
                        logoDescriptor.FitArea();
                    });
                    page.Content().Column(col =>
                    {
                        col.Item().Text("Technician Info").FontSize(14).Bold();
                        col.Item().Text($"Specialty: {tech.Specialty}");
                        col.Item().Text($"Employment: {tech.EmploymentDate?.ToShortDateString() ?? "-"}");
                        col.Item().Text($"Skills: {string.Join(", ", tech.Skills)}");
                        if (!string.IsNullOrEmpty(tech.Badges))
                            col.Item().Text($"Badges: {tech.Badges}");
                        if (!string.IsNullOrEmpty(tech.PhotoUrl))
                        {
                            var photoDescriptor = col.Item().Image(tech.PhotoUrl);
                            photoDescriptor.FitWidth();
                        }
                        col.Item().PaddingVertical(10);
                        col.Item().Text("KPI Summary").FontSize(14).Bold();
                        col.Item().Text($"Close Rate: {tech.CloseRate:P2}");
                        col.Item().Text($"Completed Jobs: {tech.CompletedJobs}");
                        col.Item().Text($"Callbacks: {tech.Callbacks}");
                        col.Item().Text($"Avg Review: {tech.AvgReviewScore:F2} ({tech.ReviewCount} reviews)");
                        col.Item().PaddingVertical(10);
                        col.Item().Text("Charts").FontSize(14).Bold();
                        if (chartImage != null && chartImage.Length > 0)
                        {
                            var chartDescriptor = col.Item().Image(chartImage);
                            chartDescriptor.FitWidth();
                        }
                        col.Item().PaddingVertical(10);
                        col.Item().Text("HR Notes").FontSize(14).Bold();
                        col.Item().Text(notes ?? "");
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GenerateComparisonReport(List<TechnicianProfileDto> techs, List<byte[]> chartImages, string? notes)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Technician Comparison Report").FontSize(20).Bold();
                            col.Item().Text(string.Join(", ", techs.ConvertAll(t => t.FullName))).FontSize(14);
                        });
                        var logoDescriptor = row.ConstantItem(80).Image("wwwroot/img/logo.png");
                        logoDescriptor.FitArea();
                    });
                    page.Content().Column(col =>
                    {
                        col.Item().Text("Technician Info").FontSize(14).Bold();
                        foreach (var tech in techs)
                        {
                            col.Item().Text($"{tech.FullName}: {tech.Specialty}, {tech.EmploymentDate?.ToShortDateString() ?? "-"}, Skills: {string.Join(", ", tech.Skills)}");
                        }
                        col.Item().PaddingVertical(10);
                        col.Item().Text("KPI Summary").FontSize(14).Bold();
                        foreach (var tech in techs)
                        {
                            col.Item().Text($"{tech.FullName}: Close Rate: {tech.CloseRate:P2}, Completed: {tech.CompletedJobs}, Callbacks: {tech.Callbacks}, Avg Review: {tech.AvgReviewScore:F2} ({tech.ReviewCount})");
                        }
                        col.Item().PaddingVertical(10);
                        col.Item().Text("Charts").FontSize(14).Bold();
                        foreach (var img in chartImages)
                        {
                            if (img != null && img.Length > 0)
                            {
                                var chartDescriptor = col.Item().Image(img);
                                chartDescriptor.FitWidth();
                            }
                        }
                        col.Item().PaddingVertical(10);
                        col.Item().Text("HR Notes").FontSize(14).Bold();
                        col.Item().Text(notes ?? "");
                    });
                });
            }).GeneratePdf();
        }
    }
}
