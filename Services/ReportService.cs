using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BrokenCode.Etc;
using BrokenCode.Extensions.DatabaseExtensions;
using BrokenCode.Interfaces;
using BrokenCode.Model;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrokenCode
{
    public class ReportService : IReportService
    {
        private const int GetReportTimeOut = 10 * 60;

        private readonly UserDbContext _db;
        private readonly ILicenseService _licenseService;

        private static readonly ILog Log = LogManager.GetLogger(typeof(ReportService));

        public ReportService(UserDbContext db, ILicenseService licenseService)
        {
            _db = db;
            _licenseService = licenseService;
        }

        public async Task<IActionResult> GetReportAsync(GetReportRequest request)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource(GetReportTimeOut);
            CancellationToken token = cancelTokenSource.Token;

            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();

                return await GetReportInnerAsync(request, token);
            }
            catch(OperationCanceledException)
            {
                Log.Debug($"Attempt {stopWatch.Elapsed} failed");
                return new StatusCodeResult(500);
            }
            finally
            {
                cancelTokenSource.Dispose();
            }
        }

        private async Task<IActionResult> GetReportInnerAsync(GetReportRequest request, CancellationToken token)
        {
            Thread.Sleep(10000);
            
            var usersOnPage =
                await _db.Users.HaveBackupsInDomainAsync(request.DomainId, request.PageNumber, request.PageSize);

            if (token.IsCancellationRequested)
            {
                return PartialResult();
            }

            // TODO: Can move to other request?
            await _licenseService.LogTotalLicensesCountForDomain(request.DomainId);

            if (token.IsCancellationRequested)
            {
                return PartialResult();
            }

            var usersData = usersOnPage
                .Select(u => new UserStatistics(u, _licenseService.GetLicenseTypeForUser(u.Id)));

            if (token.IsCancellationRequested)
            {
                return PartialResult(usersData);
            }

            var totalCountUsersHaveBackups = await _db.Users.HaveBackupsInDomainCountAsync(request.DomainId);
            return new OkObjectResult(new
            {
                TotalCount = totalCountUsersHaveBackups,
                Data = usersData
            });
        }

        private static IActionResult PartialResult(IEnumerable<UserStatistics> data = null, int totalCount = 0)
        {
            return new OkObjectResult(new
            {
                TotalCount = 0,
                Data = data ?? new List<UserStatistics>()
            });
        }
    }
}