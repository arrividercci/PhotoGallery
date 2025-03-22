using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage.Abstraction;
using System.Security.Claims;
using WebServer.API.Helpers;
using WebServer.Application.Interfaces;
using WebServer.Application.Models;
using WebServer.Domain.Entities;

namespace WebServer.API.Controllers
{
    [Route("api/albums")]
    [ApiController]
    public class AlbumsController(IAlbumService albumService, IStorageService storageService, IUserService userService, AzureBlobConfig azureBlobConfig, ILogger<AlbumsController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            var albums = await albumService.GetAlbumsAsync();
            return Ok(albums);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetAsync(int id)
        {
            var album = await albumService.GetAlbumAsync(id);
            if (album == null)
            {
                return NotFound($"Not found album with Id={id}");
            }

            return Ok(album);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateAsync(AlbumCreateModel albumCreateModel)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await userService.GetByIdAsync(userId!);
            var album = await albumService.CreateAlbumAsync(albumCreateModel, user);
            return Ok(album);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> RemoveAsync(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await userService.GetByIdAsync(userId!);
            var album = await albumService.GetAlbumAsync(id);
            if (album == null)
            {
                return NotFound($"Not found album with Id={id}");
            }

            var isUserAdmin = await userService.IsInRoleAsync(user, UserRoles.Admin);
            if (album.AuthorId == user.Id || isUserAdmin)
            {
                await albumService.DeleteAlbumAsync(id);
                return NoContent();
            }

            return Forbid("User doesn't have rights for the operation.");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] AlbumUpdateModel albumUpdateModel)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await userService.GetByIdAsync(userId!);
            var album = await albumService.GetAlbumAsync(id);
            if (album == null)
            {
                return NotFound($"Not found album with Id={id}");
            }

            var isUserAdmin = await userService.IsInRoleAsync(user, UserRoles.Admin);
            if (album.AuthorId == user.Id || isUserAdmin)
            {
                var updatedAlbum = await albumService.UpdateAlbumAsync(id, albumUpdateModel);
                return Ok(updatedAlbum);
            }

            return Forbid("User doesn't have rights for the operation.");
        }

        [HttpPut("{id}/images")]
        [Authorize]
        public async Task<ActionResult> AddImageAsync(int id, [FromForm] FormFile file)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await userService.GetByIdAsync(userId!);
            var album = await albumService.GetAlbumAsync(id);
            if (album == null)
            {
                return NotFound($"Not found album with Id={id}");
            }

            var isUserAdmin = await userService.IsInRoleAsync(user, UserRoles.Admin);
            if (album.AuthorId == user.Id || isUserAdmin)
            {
                var storageObject = await storageService.UploadFileAsync(file, azureBlobConfig.ContainerName);
                var image = new Image()
                {
                    Key = file.FileName,
                    Url = storageObject.Url,
                    ContentType = storageObject.ContentType,
                };

                var updatedAlbum = await albumService.AddImageToAlbumAsync(id, image);
                return Ok(updatedAlbum);
            }

            return Forbid("User doesn't have rights for the operation.");
        }

        [HttpPut("{id}/images/{imageId}")]
        [Authorize]
        public async Task<ActionResult> RemoveImageAsync(int id, int imageId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await userService.GetByIdAsync(userId!);
            var album = await albumService.GetAlbumAsync(id);
            if (album == null)
            {
                return NotFound($"Not found album with Id={id}");
            }

            var isUserAdmin = await userService.IsInRoleAsync(user, UserRoles.Admin);
            if (album.AuthorId == user.Id || isUserAdmin)
            {
                var image = album.Images.FirstOrDefault(i => i.Id == imageId);
                if (image == null)
                {
                    return NotFound($"Not found image with Id={imageId}");
                }

                var updatedAlbum = await albumService.RemoveImageFromAlbumAsync(id, imageId);
                await storageService.DeleteFileAsync(image.Key, azureBlobConfig.ContainerName);
                return Ok(updatedAlbum);
            }

            return Forbid("User doesn't have rights for the operation.");
        }
    }
}
