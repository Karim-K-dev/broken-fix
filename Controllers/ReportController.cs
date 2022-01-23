using System;
using System.Threading.Tasks;
using BrokenCode.Etc;
using Microsoft.AspNetCore.Mvc;

namespace BrokenCode.Controllers
{
    public class ReportController : Controller
    {
        private readonly IBrokenService _brokenService;
        
        public ReportController(IBrokenService brokenService)
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

            return await _brokenService.GetReport(reportRequest);
        }
    }
}