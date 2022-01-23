using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrokenCode.Etc;
using BrokenCode.Model;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BrokenCode.Interfaces
{
    public class LicenseService : ILicenseService
    {
        public LicenseServiceSettings Settings { get; }

        public IReadOnlyDictionary<Guid, LicenseInfo> LicensesInfoByUser => _licensesInfoByUser;
        private readonly Dictionary<Guid, LicenseInfo> _licensesInfoByUser = new Dictionary<Guid, LicenseInfo>();

        private static readonly ILog Log = LogManager.GetLogger(typeof(LicenseService));

        private readonly UserDbContext _userDbContext;

        public LicenseService(IOptions<LicenseServiceSettings> licenseServiceSettingsOptions,
            UserDbContext userDbContext)
        {
            Settings = licenseServiceSettingsOptions.Value;
            _userDbContext = userDbContext;
        }

        public LicenseType GetLicenseTypeForUser(Guid userId)
        {
            var licenseInfo = _licensesInfoByUser.GetValueOrDefault(userId);
            return licenseInfo?.Type ?? LicenseType.None;
        }

        public async Task<int> GetLicensedUserCountAsync(Guid domainId)
        {
            var countLicensedUsersInDomain = 0;

            var users = _userDbContext.Users
                .Where(u => u.DomainId == domainId).AsAsyncEnumerable();

            await foreach (var user in users)
            {
                if (_licensesInfoByUser.ContainsKey(user.Id))
                {
                    countLicensedUsersInDomain++;
                }
            }

            return countLicensedUsersInDomain;
        }

        public async Task LogTotalLicensesCountForDomain(Guid domainId)
        {
            Log.Info($"Total licenses for domain '{domainId}': {await GetLicensedUserCountAsync(domainId)}");
        }

        public void Dispose()
        {
        }
    }
}