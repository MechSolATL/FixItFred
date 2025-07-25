using MVP_Core.Services;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    /// <summary>
    /// PageModel for ViewMedia. Injects MediaUploadService via DI.
    /// </summary>
    public class ViewMediaModel : PageModel
    {
        private readonly MediaUploadService _mediaService;
        public List<TechnicianMedia> MediaList { get; private set; } = new();
        public int TechId { get; private set; }
        public int RequestId { get; private set; }
        public string? FilterType { get; private set; }

        public ViewMediaModel(MediaUploadService mediaService)
        {
            _mediaService = mediaService;
        }

        public void OnGet()
        {
            TechId = int.TryParse(Request.Query["techId"], out var tId) ? tId : 0;
            RequestId = int.TryParse(Request.Query["requestId"], out var rId) ? rId : 0;
            FilterType = Request.Query["type"];
            if (RequestId > 0)
                MediaList = _mediaService.GetMediaForRequest(RequestId);
            else if (TechId > 0)
                MediaList = _mediaService.GetMediaForTechnician(TechId);
            else
                MediaList = new List<TechnicianMedia>();
            if (!string.IsNullOrEmpty(FilterType))
                MediaList = MediaList.Where(m => m.FileType == FilterType).ToList();
        }
    }
}
