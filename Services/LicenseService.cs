using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrokenCode.Etc;
using Microsoft.Extensions.Options;

namespace BrokenCode.Interfaces
{
    public class LicenseService : ILicenseService
    {
        public LicenseServiceSettings Settings { get; }

        public LicenseService(IOptions<LicenseServiceSettings> licenseServiceSettingsOptions)
        {
            Settings = licenseServiceSettingsOptions.Value;
        }

        public Task<ICollection<LicenseInfo>> GetLicensesAsync(Guid domainId, ICollection<string> emails)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetLicensedUserCountAsync(Guid domainId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}