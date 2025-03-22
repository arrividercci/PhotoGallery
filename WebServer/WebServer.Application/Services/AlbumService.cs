using Microsoft.Extensions.Logging;
using WebServer.Application.Interfaces;
using WebServer.Application.Mappers;
using WebServer.Application.Models;
using WebServer.Domain.Entities;
using WebServer.Infrastructure.Interfaces;

namespace WebServer.Application.Services
{
    public class AlbumService(IAlbumRepository albumRepository, ILogger<AlbumService> logger) : IAlbumService
    {
        public async Task<AlbumModel> AddImageToAlbumAsync(int id, Image image)
        {
            logger.LogInformation("Adding image to album. AlbumId={AlbumId}, ImageId={ImageId}", id, image.Id);

            var album = await albumRepository.GetAlbumWithImagesAsync(id);
            if (album == null)
            {
                logger.LogWarning("Album not found. AlbumId={AlbumId}", id);
                throw new ArgumentNullException($"Album with Id={id} not found.");
            }

            album.Images.Add(image);
            await albumRepository.SaveChangesAsync();

            logger.LogInformation("Image added successfully to album. AlbumId={AlbumId}, ImageId={ImageId}", id, image.Id);
            return album.ToModel()!;
        }

        public async Task<AlbumModel> CreateAlbumAsync(AlbumCreateModel model, User author)
        {
            logger.LogInformation("Creating a new album. AuthorId={AuthorId}, AlbumName={AlbumName}", author.Id, model.Name);

            var album = model.ToEntity(author);
            await albumRepository.AddAsync(album);
            await albumRepository.SaveChangesAsync();

            logger.LogInformation("Album created successfully. AlbumId={AlbumId}, AuthorId={AuthorId}", album.Id, author.Id);
            return album.ToModel();
        }

        public async Task DeleteAlbumAsync(int id)
        {
            logger.LogInformation("Deleting album. AlbumId={AlbumId}", id);

            var album = await albumRepository.GetByAsync(id);
            if (album == null)
            {
                logger.LogWarning("Album not found. AlbumId={AlbumId}", id);
                throw new ArgumentNullException($"Album with Id={id} not found.");
            }

            albumRepository.Delete(album);
            await albumRepository.SaveChangesAsync();

            logger.LogInformation("Album deleted successfully. AlbumId={AlbumId}", id);
        }

        public async Task<AlbumModel?> GetAlbumAsync(int id)
        {
            logger.LogInformation("Fetching album details. AlbumId={AlbumId}", id);

            var album = await albumRepository.GetAlbumWithImagesAsync(id);
            if (album == null)
            {
                logger.LogWarning("Album not found. AlbumId={AlbumId}", id);
            }

            return album?.ToModel();
        }

        public async Task<List<AlbumModel>> GetAlbumsAsync()
        {
            logger.LogInformation("Fetching all albums.");

            var albums = await albumRepository.GetAlbumsWithImagesAsync();
            return albums.Select(album => album.ToModel()).ToList();
        }

        public async Task<List<AlbumModel>> GetUserAlbumsAsync(string userId)
        {
            logger.LogInformation("Fetching albums for user. UserId={UserId}", userId);

            var albums = await albumRepository.GetUserAlbumsWithImagesAsync(userId);
            return albums.Select(album => album.ToModel()).ToList();
        }

        public async Task<AlbumModel> RemoveImageFromAlbumAsync(int id, int imageId)
        {
            logger.LogInformation("Removing image from album. AlbumId={AlbumId}, ImageId={ImageId}", id, imageId);

            var album = await albumRepository.GetAlbumWithImagesAsync(id);
            if (album == null)
            {
                logger.LogWarning("Album not found. AlbumId={AlbumId}", id);
                throw new ArgumentNullException($"Album with Id={id} not found.");
            }

            var image = album.Images.FirstOrDefault(image => image.Id == imageId);
            if (image == null)
            {
                logger.LogWarning("Image not found in album. AlbumId={AlbumId}, ImageId={ImageId}", id, imageId);
                throw new ArgumentNullException($"Image with Id={imageId} not found in album {id}.");
            }

            album.Images.Remove(image);
            await albumRepository.SaveChangesAsync();

            logger.LogInformation("Image removed successfully from album. AlbumId={AlbumId}, ImageId={ImageId}", id, imageId);
            return album.ToModel();
        }

        public async Task<AlbumModel> UpdateAlbumAsync(int id, AlbumUpdateModel model)
        {
            var album = await albumRepository.GetAlbumWithImagesAsync(id);
            if (album == null)
            {
                logger.LogWarning("Album not found. AlbumId={AlbumId}", id);
                throw new ArgumentNullException($"Album with Id={id} not found.");
            }

            var updatedAlbum = model.ToEntity(album);
            albumRepository.Update(updatedAlbum);
            await albumRepository.SaveChangesAsync();

            logger.LogInformation($"Album updated successfully. AlbumId={id}");
            return album.ToModel();
        }
    }

}
