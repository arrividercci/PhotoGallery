using WebServer.Application.Models;
using WebServer.Domain.Entities;

namespace WebServer.Application.Mappers
{
    public static class ImageExtensions
    {
        public static ImageModel ToModel(this Image image)
        {
            return new ImageModel
            {
                Id = image.Id,
                ContentType = image.ContentType,
                Key = image.Key,
                Url = image.Url
            };
        }
    }
}
