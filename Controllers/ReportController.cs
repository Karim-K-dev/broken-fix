using System;
using System.Threading.Tasks;
using BrokenCode.Etc;
using Microsoft.AspNetCore.Mvc;

namespace BrokenCode.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _brokenService;
        
        public ReportController(IReportService brokenService)
        {
            _brokenService = brokenService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetReport(GetReportRequest reportRequest)
        {
            if (reportRequest == null)
            {
                throw new ArgumentNullException(nameof(reportRequest));
            }

            return await _brokenService.GetReportAsync(reportRequest);
        }
    }
}