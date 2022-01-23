using BrokenCode.Etc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrokenCode.Model;

namespace BrokenCode.Interfaces
{
    public interface ILicenseService : IDisposable
    {
        LicenseServiceSettings Settings { get; }
        IReadOnlyDictionary<Guid, LicenseInfo> LicensesInfoByUser { get; }
        
        Task<int> GetLicensedUserCountAsync(Guid domainId);
        
        Task LogTotalLicensesCountForDomain(Guid domainId);
    }
}
