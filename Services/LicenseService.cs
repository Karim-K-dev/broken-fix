using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrokenCode.Etc;
using BrokenCode.Model;
using log4net;
using Microsoft.Extensions.Options;

namespace BrokenCode.Interfaces
{
    public class LicenseService : ILicenseService
    {
        public LicenseServiceSettings Settings { get; }

        // TODO: Use dictionary this for improve access performance (there can be a lot of info about licenses)
        private readonly List<LicenseInfo> _licensesInfo = new List<LicenseInfo>();
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(LicenseService));

        public LicenseService(IOptions<LicenseServiceSettings> licenseServiceSettingsOptions)
        {
            Settings = licenseServiceSettingsOptions.Value;
        }
        
        public async Task<IDictionary<Guid, LicenseInfo>> GetUserLicensesAsync(ICollection<User> users)
        {
            // TODO: Select by users.
            return new Dictionary<Guid, LicenseInfo>();
        }

        public async Task<int> GetLicensedUserCountAsync(Guid domainId)
        {
            // TODO: Select  domainId only.
            return _licensesInfo.Count;
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