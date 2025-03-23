using Microsoft.Extensions.Logging;
using WebServer.Infrastructure.Interfaces;
using WebServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebServer.Infrastructure.Repositories
{
    public class ImageRepository(PhotoGalleryDbContext dbContext, ILogger<ImageRepository> logger) : Repository<Image, int>(dbContext, logger), IImageRepository
    {
        public async Task<Image?> GetImageWithReactsAsync(int id)
        {
            return await AllData.Include(image => image.Reactions).FirstOrDefaultAsync();
        }
    }
}
