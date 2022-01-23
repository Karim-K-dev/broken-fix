using System;

namespace BrokenCode.Interfaces
{
    public class LicenseServiceProvider : ILicenseServiceProvider
    {
        public ILicenseService GetLicenseService()
        {
            throw new NotImplementedException();
        }
    }
}