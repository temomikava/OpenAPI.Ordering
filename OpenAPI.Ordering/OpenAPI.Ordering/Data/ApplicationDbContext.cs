using Microsoft.EntityFrameworkCore;
using OpenAPI.Ordering.Data;
using OpenAPI.Ordering;

namespace OpenAPI.Identity.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ComputationResult> ComputationResults { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().HasKey(x => x.Id);
            modelBuilder.Entity<ComputationResult>().HasKey(x => x.Id);
            modelBuilder.Entity<Order>().HasKey(x => x.Id);
            modelBuilder.Entity<Order>().HasOne(x => x.Company).WithMany(x => x.Orders).HasForeignKey(x => x.CompanyId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
