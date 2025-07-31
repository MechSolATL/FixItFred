using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Data;
using Data.Models;

namespace Pages.Admin
{
    public class TechReportCardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<Data.Models.Technician> TechnicianList { get; set; } = new();
        public List<TechnicianReportCard> ReportCards { get; set; } = new();
        public int? SelectedTechId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SelectedSeverity { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; } = 1;
        public List<TechnicianReportCard> AlertTechnicians { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public TechReportCardModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync(int? TechId, DateTime? StartDate, DateTime? EndDate, string? Severity, int PageNumber = 1, string? export = null)
        {
            try
            {
                SelectedTechId = TechId;
                this.StartDate = StartDate;
                this.EndDate = EndDate;
                SelectedSeverity = Severity;
                this.PageNumber = PageNumber;
                TechnicianList = await _db.Technicians.ToListAsync();
                var service = new TechnicianReportCardService(_db);
                var allCards = await Task.Run(() => service.GetAllReportCards());
                if (TechId.HasValue)
                    allCards = allCards.Where(x => x.TechnicianId == TechId.Value).ToList();
                if (StartDate.HasValue)
                    allCards = allCards.Where(x => x.Notes == null || x.Notes.Contains(StartDate.Value.ToString("yyyy-MM-dd"))).ToList(); // Placeholder for date filter
                if (EndDate.HasValue)
                    allCards = allCards.Where(x => x.Notes == null || x.Notes.Contains(EndDate.Value.ToString("yyyy-MM-dd"))).ToList(); // Placeholder for date filter
                if (!string.IsNullOrEmpty(Severity))
                    allCards = allCards.Where(x => x.SevereLateClockInCount > 0 && Severity == "Severe" || x.LateClockInCount > 0 && Severity == "Warning").ToList();
                TotalPages = (int)Math.Ceiling(allCards.Count / (double)PageSize);
                ReportCards = allCards.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
                AlertTechnicians = allCards.Where(x => x.SevereLateClockInCount >= 2 || x.TrustScore < 5).ToList();
                if (export == "csv")
                {
                    var csv = new StringBuilder();
                    csv.AppendLine("Technician,LateClockIns,SevereLate,IdleMinutes,EgoScore,ConfidenceShift,Pulse,Trust,Notes");
                    foreach (var card in allCards)
                    {
                        csv.AppendLine($"{card.TechnicianName},{card.LateClockInCount},{card.SevereLateClockInCount},{card.IdleMinutesWeek},{card.EgoInfluenceScore},{card.ConfidenceShift},{card.PulseScore},{card.TrustScore},{card.Notes}");
                    }
                    Response.Headers.Append("Content-Disposition", "attachment; filename=TechReportCard.csv");
                    Response.ContentType = "text/csv";
                    await Response.Body.WriteAsync(Encoding.UTF8.GetBytes(csv.ToString()));
                }
                else if (export == "excel")
                {
                    using var package = new ExcelPackage();
                    var ws = package.Workbook.Worksheets.Add("ReportCard");
                    ws.Cells[1, 1].Value = "Technician";
                    ws.Cells[1, 2].Value = "LateClockIns";
                    ws.Cells[1, 3].Value = "SevereLate";
                    ws.Cells[1, 4].Value = "IdleMinutes";
                    ws.Cells[1, 5].Value = "EgoScore";
                    ws.Cells[1, 6].Value = "ConfidenceShift";
                    ws.Cells[1, 7].Value = "Pulse";
                    ws.Cells[1, 8].Value = "Trust";
                    ws.Cells[1, 9].Value = "Notes";
                    int row = 2;
                    foreach (var card in allCards)
                    {
                        ws.Cells[row, 1].Value = card.TechnicianName;
                        ws.Cells[row, 2].Value = card.LateClockInCount;
                        ws.Cells[row, 3].Value = card.SevereLateClockInCount;
                        ws.Cells[row, 4].Value = card.IdleMinutesWeek;
                        ws.Cells[row, 5].Value = card.EgoInfluenceScore;
                        ws.Cells[row, 6].Value = card.ConfidenceShift;
                        ws.Cells[row, 7].Value = card.PulseScore;
                        ws.Cells[row, 8].Value = card.TrustScore;
                        ws.Cells[row, 9].Value = card.Notes;
                        row++;
                    }
                    Response.Headers.Append("Content-Disposition", "attachment; filename=TechReportCard.xlsx");
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    await Response.Body.WriteAsync(package.GetAsByteArray());
                }
                else if (export == "pdf")
                {
                    var doc = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(20);
                            page.Header().Text("Technician Behavior Report Card").FontSize(18).Bold();
                            page.Content().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(100);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(60);
                                    columns.ConstantColumn(60);
                                    columns.RelativeColumn();
                                });
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Technician");
                                    header.Cell().Element(CellStyle).Text("LateClockIns");
                                    header.Cell().Element(CellStyle).Text("SevereLate");
                                    header.Cell().Element(CellStyle).Text("IdleMinutes");
                                    header.Cell().Element(CellStyle).Text("EgoScore");
                                    header.Cell().Element(CellStyle).Text("ConfidenceShift");
                                    header.Cell().Element(CellStyle).Text("Pulse");
                                    header.Cell().Element(CellStyle).Text("Trust");
                                    header.Cell().Element(CellStyle).Text("Notes");
                                });
                                foreach (var card in allCards)
                                {
                                    table.Cell().Element(CellStyle).Text(card.TechnicianName);
                                    table.Cell().Element(CellStyle).Text(card.LateClockInCount.ToString());
                                    table.Cell().Element(CellStyle).Text(card.SevereLateClockInCount.ToString());
                                    table.Cell().Element(CellStyle).Text(card.IdleMinutesWeek.ToString());
                                    table.Cell().Element(CellStyle).Text(card.EgoInfluenceScore.ToString());
                                    table.Cell().Element(CellStyle).Text(card.ConfidenceShift.ToString());
                                    table.Cell().Element(CellStyle).Text(card.PulseScore.ToString());
                                    table.Cell().Element(CellStyle).Text(card.TrustScore.ToString());
                                    table.Cell().Element(CellStyle).Text(card.Notes);
                                }
                            });
                        });
                    });
                    Response.Headers.Append("Content-Disposition", "attachment; filename=TechReportCard.pdf");
                    Response.ContentType = "application/pdf";
                    await Response.Body.WriteAsync(doc.GeneratePdf());
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load report card data. Please try again or contact support.";
            }
        }
        private IContainer CellStyle(IContainer container) => container.PaddingVertical(2).PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }
}
