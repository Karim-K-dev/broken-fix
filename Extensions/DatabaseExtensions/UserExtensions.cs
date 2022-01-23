using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrokenCode.Model;
using Microsoft.EntityFrameworkCore;

namespace BrokenCode.Extensions.DatabaseExtensions
{
    public static class UserDbExtensions
    {
        public static async Task<List<User>> HaveBackupsInDomainAsync(this DbSet<User> users, Guid domainId, int pageNumber = 0, int pageSize = 10)
        {
            var request = users
                .Where(u => u.DomainId == domainId && u.BackupEnabled && u.State == UserState.InDomain) // Filter users by request.
                .Skip(pageSize * pageNumber) // Skip users at first.
                .Take(pageSize);

            return await request.ToListAsync(); 
        }

        public static async Task<int> HaveBackupsInDomainCountAsync(this DbSet<User> users, Guid domainId)
        {
            return await users.CountAsync(u =>
                u.DomainId == domainId && u.BackupEnabled && u.State == UserState.InDomain);
        }
    }
}