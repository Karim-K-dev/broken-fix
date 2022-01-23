using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrokenCode.Etc;
using log4net;
using Microsoft.Extensions.Options;

namespace BrokenCode.Interfaces
{
    public class LicenseService : ILicenseService
    {
        public LicenseServiceSettings Settings { get; }

        public IReadOnlyDictionary<Guid, LicenseInfo> LicensesInfoByUser => _licensesInfoByUser;
        private readonly Dictionary<Guid, LicenseInfo> _licensesInfoByUser = new Dictionary<Guid, LicenseInfo>();

        private static readonly ILog Log = LogManager.GetLogger(typeof(LicenseService));

        public LicenseService(IOptions<LicenseServiceSettings> licenseServiceSettingsOptions)
        {
            Settings = licenseServiceSettingsOptions.Value;
        }
        
        public string GetLicenseTypeForUser(Guid userId)
        {
            var licenseInfo = _licensesInfoByUser.GetValueOrDefault(userId);
            if (licenseInfo == null)
            {
                return "None";
            }

            return licenseInfo.IsTrial ? "Trial" : "Paid";
        }

        public async Task<int> GetLicensedUserCountAsync(Guid domainId)
        {
            // TODO: Select  domainId only.
            return LicensesInfoByUser.Count;
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