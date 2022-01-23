using System.Threading.Tasks;
using BrokenCode.Etc;
using BrokenCode.Interfaces;
using BrokenCode.Model;
using Microsoft.AspNetCore.Mvc;

namespace BrokenCode
{
    public interface IBrokenService
    {
        Task<IActionResult> GetReport(GetReportRequest request);
        Task<IActionResult> GetReportAsync(GetReportRequest request);
        bool InBackup(User user);
    }
}