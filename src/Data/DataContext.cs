using EAScraperJob.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EAScraperJob.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Property> Properties { get; set; }

        public DbSet<Audit> Audit { get; set; }

        public DbSet<Log> Log { get; set; }
    }
}
