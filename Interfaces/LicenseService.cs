using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrokenCode.Etc;

namespace BrokenCode.Interfaces
{
    public class LicenseService : ILicenseService
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<LicenseInfo>> GetLicensesAsync(Guid domainId, ICollection<string> emails)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetLicensedUserCountAsync(Guid domainId)
        {
            throw new NotImplementedException();
        }

        public LicenseServiceSettings Settings { get; set; }
    }
}