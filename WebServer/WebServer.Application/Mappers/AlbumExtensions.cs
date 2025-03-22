using WebServer.Application.Models;
using WebServer.Domain.Entities;

namespace WebServer.Application.Mappers
{
    public static class AlbumExtensions
    {
        public static AlbumModel? ToModel(this Album? album)
        {
            return album == null ? null : new AlbumModel();
        }

        public static Album ToEntity(this AlbumCreateModel albumCreateModel, User author)
        {
            return new Album()
            {
                Name = albumCreateModel.Name,
                AuthorId = author.Id,
                Author = author
            };
        }

        public static Album ToEntity(this AlbumUpdateModel albumUpdateModel, Album album)
        {
            return new Album()
            {
                Id = album.Id,
                Name = albumUpdateModel.Name,
                AuthorId = album.AuthorId,
                Images = album.Images,
            };
        }
    }
}
