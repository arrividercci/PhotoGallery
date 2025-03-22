using Microsoft.EntityFrameworkCore;
using WebServer.Domain.Entities;
using WebServer.Infrastructure.Data;

namespace WebServer.Infrastructure
{
    public class PhotoGalleryDbContext(DbContextOptions options) : ApplicationIdentityDbContext<User>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreating<PhotoGalleryDbContext>(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
