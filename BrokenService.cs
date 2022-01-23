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
        private readonly ILicenseService _licenseService;

        private static readonly ILog Log = LogManager.GetLogger(typeof(BrokenService));

        public BrokenService(UserDbContext db, ILicenseService licenseService)
        {
            _db = db;
            _licenseService = licenseService;
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
            var usersOnPage = await GetUsersHaveBackupsInDomain(request.DomainId, request.PageNumber, request.PageSize);

            // TODO: Can move to other request?
            await _licenseService.LogTotalLicensesCountForDomain(request.DomainId);
            
            var userLicenses = await _licenseService.GetUserLicensesAsync(usersOnPage);

            // получаем тип лицензии для пользователя и все вставляем в польщовательскую статистику
            // нужно упростить пользовательскую статистику
            var usersData = (usersOnPage.ToList())
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

            var totalCountUsersHaveBackups = await GetTotalUsersHaveBackupsInDomain(request.DomainId);
            return new OkObjectResult(new
            {
                TotalCount = totalCountUsersHaveBackups,
                Data = usersData
            });
        }

        public async Task<List<User>> GetUsersHaveBackupsInDomain(Guid domainId, int pageNumber = 0, int pageSize = 10)
        {
            var request = _db.Users
                .Where(u => HaveBackupInDomain(u, domainId)) // Filter users by request.
                .Skip(pageSize * pageNumber) // Skip users at first.
                .Take(pageSize);

            return await request.ToListAsync();
        }

        public async Task<int> GetTotalUsersHaveBackupsInDomain(Guid domainId)
        {
            return await _db.Users.CountAsync(u => HaveBackupInDomain(u, domainId));
        }

        public bool HaveBackupInDomain(User user, Guid domainId)
        {
            return user.DomainId == domainId && InBackup(user);
        }

        public bool InBackup(User user)
        {
            return user.BackupEnabled && user.State == UserState.InDomain;
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