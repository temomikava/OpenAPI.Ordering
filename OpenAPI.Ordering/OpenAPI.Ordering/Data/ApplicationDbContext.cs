using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System;
using OpenAPI.Ordering.Data;

namespace OpenAPI.Identity.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().HasKey(x => x.Id);
            modelBuilder.Entity<Order>().HasKey(x => x.Id);
            modelBuilder.Entity<Order>().HasOne(x => x.Company).WithMany(x => x.Orders).HasForeignKey(x => x.CompanyId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
