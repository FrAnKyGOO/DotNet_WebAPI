using DotNet_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet_WebAPI.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;

        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }

        public virtual DbSet<UsersModels> Users { get; set; }
        public virtual DbSet<UsersJobInfoModels> UsersJobInfo { get; set; }
        public virtual DbSet<UsersSalaryModels> UsersSalary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");

            modelBuilder.Entity<UsersModels>()
                .ToTable("users", "TutorialAppSchema")
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UsersJobInfoModels>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UsersSalaryModels>()
                .HasKey(u => u.UserId);


        }
    }
}
