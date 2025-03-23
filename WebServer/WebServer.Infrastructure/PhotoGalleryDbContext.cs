using Common.Data;
using Microsoft.EntityFrameworkCore;
using WebServer.Domain.Entities;

namespace WebServer.Infrastructure
{
    public class PhotoGalleryDbContext(DbContextOptions options) : ApplicationIdentityDbContext<User>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreating<PhotoGalleryDbContext>(modelBuilder);

            modelBuilder.Entity<UserReaction>()
                .HasKey(userReaction => new { userReaction.UserId, userReaction.ImageId });

            modelBuilder.Entity<UserReaction>()
                        .HasOne(userReaction => userReaction.Image)
                        .WithMany(image => image.Reactions)
                        .HasForeignKey(userReaction => userReaction.ImageId);

            modelBuilder.Entity<UserReaction>()
                        .HasOne(userReaction => userReaction.User)
                        .WithMany(user => user.Reactions)
                        .HasForeignKey(userReaction => userReaction.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
