using Microsoft.EntityFrameworkCore;

namespace Common.Data.Tests
{
    public class TestDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<TestItem> Items { get; set; }
    }
}
