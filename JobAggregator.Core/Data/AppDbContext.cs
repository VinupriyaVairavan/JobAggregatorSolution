
using Microsoft.EntityFrameworkCore;
using JobAggregator.Core.Models;

namespace JobAggregator.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<Application> Applications { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobPosting>()
                .HasIndex(j => j.Link)
                .IsUnique();
        }
    }
}
