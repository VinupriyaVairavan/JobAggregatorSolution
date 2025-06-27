
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using JobAggregator.Core.Data;

namespace JobAggregator.Core.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            // Use a dummy connection string for design-time. It won't actually connect.
            optionsBuilder.UseSqlite("DataSource=design_time.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
