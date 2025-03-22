using WebServer.Domain.Entities;

namespace WebServer.Infrastructure.Interfaces
{
    public interface IAlbumRepository : IRepository<Album, int>
    {
        Task<Album?> GetAlbumWithImagesAsync(int albumId);
        Task<List<Album>> GetAlbumsWithImagesAsync();
        Task<List<Album>> GetUserAlbumsWithImagesAsync(string userId);
    }
}
