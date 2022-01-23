using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BrokenCode.Etc;
using BrokenCode.Interfaces;
using BrokenCode.Model;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrokenCode
{
    public class BrokenService : IBrokenService
    {
        private const int GetReportTimeOut = 10 * 60;

        // TODO: Insert to DI container.
        private readonly UserDbContext _db;
        private readonly ILicenseServiceProvider _licenseServiceProvider;

        private static readonly ILog Log = LogManager.GetLogger(typeof(BrokenService));

        public BrokenService(UserDbContext db, ILicenseServiceProvider licenseServiceProvider)
        {
            _db = db;
            _licenseServiceProvider = licenseServiceProvider;
        }

        public async Task<IActionResult> GetReport(GetReportRequest request)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            
            var taskTimeOutObserver = new Task(() =>
            {
                Thread.Sleep(GetReportTimeOut);
                cancelTokenSource.Cancel();
            });
            
            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                taskTimeOutObserver.Start();
                
                return await GetReportAsync(request);
            }
            catch
            {
                Log.Debug($"Attempt {stopWatch.Elapsed} failed");
                return new StatusCodeResult(500);
            }
            finally
            {
                cancelTokenSource.Dispose();
            }
        }

        // TODO: Split this big method.
        public async Task<IActionResult> GetReportAsync(GetReportRequest request)
        {
            var filteredUsers = _db.Users
                .Where(u => InBackup(u) && u.DomainId == request.DomainId);

            var totalCount = await filteredUsers.CountAsync();
            
            var usersOnPage = filteredUsers.Take(request.PageSize).Skip(request.PageSize * request.PageNumber);

            var userLicenses = new Dictionary<Guid, LicenseInfo>();
            using var licenseService = GetLicenseServiceAndConfigure();

            if (licenseService != null)
            {
                Log.Info(
                    $"Total licenses for domain '{request.DomainId}': {licenseService.GetLicensedUserCountAsync(request.DomainId)}");

                var emails = await usersOnPage.Select(u => u.UserEmail).ToListAsync();
                ICollection<LicenseInfo> result = null;

                try
                {
                    result = await licenseService.GetLicensesAsync(request.DomainId, emails);
                }
                catch (Exception ex)
                {
                    Log.Error($"Problem of getting licenses information: {ex.Message}");
                    throw;
                }

                if (result != null)
                {
                    foreach (User user in usersOnPage)
                    {
                        if (result.Count(r => r.Email == user.UserEmail) > 0)
                        {
                            userLicenses.Add(user.Id, result.Where(r => r.Email == user.UserEmail).First());
                        }
                    }
                }
            }

            var usersData = (await usersOnPage.ToListAsync())
                .Select(u =>
                {
                    string licenseType = userLicenses.ContainsKey(u.Id)
                        ? (userLicenses[u.Id].IsTrial ? "Trial" : "Paid") // Move to constants.
                        : "None";

                    return new UserStatistics
                    {
                        Id = u.Id,
                        UserName = u.UserEmail,
                        InBackup = u.BackupEnabled,
                        EmailLastBackupStatus = u.Email.LastBackupStatus,
                        EmailLastBackupDate = u.Email.LastBackupDate,
                        DriveLastBackupStatus = u.Drive.LastBackupStatus,
                        DriveLastBackupDate = u.Drive.LastBackupDate,
                        CalendarLastBackupStatus = u.Calendar.LastBackupStatus,
                        CalendarLastBackupDate = u.Calendar.LastBackupDate,
                        LicenseType = licenseType
                    };
                });

            return new OkObjectResult(new
            {
                TotalCount = totalCount,
                Data = usersData
            });
        }

        public bool InBackup(User user)
        {
            return user.BackupEnabled && user.State == UserState.InDomain;
        }

        public ILicenseService GetLicenseServiceAndConfigure()
        {
            using var result = _licenseServiceProvider.GetLicenseService();

            Configure(result.Settings);

            return result;
        }

        // TODO: Get _licenseServiceProvider from DI.
        private void Configure(LicenseServiceSettings settings)
        {
            if (settings != null)
            {
                settings.TimeOut = 5000;
            }
            else
            {
                settings = new LicenseServiceSettings
                {
                    TimeOut = 5000
                };
            }
        }
    }
}
