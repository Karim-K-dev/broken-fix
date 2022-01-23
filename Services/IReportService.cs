using System.Threading.Tasks;
using BrokenCode.Etc;
using Microsoft.AspNetCore.Mvc;

namespace BrokenCode
{
    public interface IReportService
    {
        Task<IActionResult> GetReportAsync(GetReportRequest request);
    }
}