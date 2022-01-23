using System.Threading.Tasks;
using BrokenCode.Etc;
using Microsoft.AspNetCore.Mvc;

namespace BrokenCode
{
    public interface IBrokenService
    {
        Task<IActionResult> GetReportAsync(GetReportRequest request);
    }
}