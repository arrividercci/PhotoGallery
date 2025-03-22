using WebServer.Application.Models;
using WebServer.Domain.Entities;

namespace WebServer.Application.Interfaces
{
    public interface IAlbumService
    {
        Task<AlbumModel?> GetAlbumAsync(int id);
        Task<List<AlbumModel>> GetUserAlbumsAsync(string userId);
        Task<List<AlbumModel>> GetAlbumsAsync();
        Task<AlbumModel> CreateAlbumAsync(AlbumCreateModel model, User author);
        Task DeleteAlbumAsync(int id);
        Task<AlbumModel> AddImageToAlbumAsync(int id, Image image);
        Task<AlbumModel> RemoveImageFromAlbumAsync(int id, int imageId);
        Task<AlbumModel> UpdateAlbumAsync(int id, AlbumUpdateModel model);
    }
}
