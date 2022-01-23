using System;
using BrokenCode.Model;

namespace BrokenCode.Etc
{
    public class UserStatistics
    {
        public Guid Id { get; }
        public string UserName { get; }
        public bool InBackup { get; }
        public string EmailLastBackupStatus { get; }
        public DateTime? EmailLastBackupDate { get; }
        public string DriveLastBackupStatus { get; }
        public DateTime? DriveLastBackupDate { get; }
        public string CalendarLastBackupStatus { get; }
        public DateTime? CalendarLastBackupDate { get; }
        public string LicenseType { get; }

        public UserStatistics(User user, LicenseType licenseType)
        {
            Id = user.Id;
            UserName = user.UserEmail;
            InBackup = user.BackupEnabled;
            EmailLastBackupStatus = user.Email?.LastBackupStatus;
            EmailLastBackupDate = user.Email?.LastBackupDate;
            DriveLastBackupStatus = user.Drive?.LastBackupStatus;
            DriveLastBackupDate = user.Drive?.LastBackupDate;
            CalendarLastBackupStatus = user.Calendar?.LastBackupStatus;
            CalendarLastBackupDate = user.Calendar?.LastBackupDate;
            LicenseType = licenseType.ToString();
        }
    }
}