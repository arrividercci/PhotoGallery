using Microsoft.Extensions.Logging;
using WebServer.Infrastructure.Interfaces;
using WebServer.Domain.Entities;

namespace WebServer.Infrastructure.Repositories
{
    public class ImageRepository(PhotoGalleryDbContext dbContext, ILogger<ImageRepository> logger) : Repository<Image, int>(dbContext, logger), IImageRepository
    {
    }
}
