using BrokenCode.Etc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BrokenCode.Model;

namespace BrokenCode.Interfaces
{
    public interface ILicenseService : IDisposable
    {
        Task<int> GetLicensedUserCountAsync(Guid domainId);

        LicenseServiceSettings Settings { get; }
        Task LogTotalLicensesCountForDomain(Guid domainId);
        Task<IDictionary<Guid, LicenseInfo>> GetUserLicensesAsync(ICollection<User> users);
    }
}
