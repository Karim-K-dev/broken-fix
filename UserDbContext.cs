using BrokenCode.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BrokenCode
{
    public class UserDbContext : DbContext
    {
        private readonly string _fileNameDb;
        
        public UserDbContext(IConfiguration configuration)
        {
            _fileNameDb = configuration
                .GetSection("UserDatabase")
                .GetSection("SqlLiteFileName").Value;

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlite($"Filename={_fileNameDb}");
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>()
                .HasIndex(u => u.UserEmail).IsUnique();

            mb.Entity<User>()
                .HasOne(e => e.Email)
                .WithOne(t => t.User)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>()
                .HasOne(e => e.Drive)
                .WithOne(t => t.User)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>()
                .HasOne(e => e.Calendar)
                .WithOne(t => t.User)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(mb);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<Drive> Drives { get; set; }

        public DbSet<Calendar> Calendars { get; set; }
    }
}
