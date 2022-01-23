using System;
using BrokenCode.Model;

namespace BrokenCode.Etc
{
    public class LicenseInfo
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public LicenseType Type => IsTrial ? LicenseType.Trial : LicenseType.Paid;

        // TODO: Remove this.
        public bool IsTrial { get; set; }
    }
}
