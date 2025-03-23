using Microsoft.Extensions.Logging;
using WebServer.Infrastructure.Interfaces;
using WebServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Common.Data;

namespace WebServer.Infrastructure.Repositories
{
    public class AlbumRepository(PhotoGalleryDbContext dbContext, ILogger<AlbumRepository> logger) : Repository<Album, int>(dbContext, logger), IAlbumRepository
    {
        public async Task<List<Album>> GetAlbumsWithImagesAsync()
        {
            return await AllData
                .Include(album => album.Images)
                .ToListAsync();
        }

        public Task<Album?> GetAlbumWithImagesAsync(int albumId)
        {
            return AllData
                .Include(album => album.Images)
                .FirstOrDefaultAsync(album => album.Id == albumId);
        }

        public Task<List<Album>> GetUserAlbumsWithImagesAsync(string userId)
        {
            return AllData
                .Include(album => album.Images)
                .Where(album => album.AuthorId == userId)
                .ToListAsync();
        }
    }
}
