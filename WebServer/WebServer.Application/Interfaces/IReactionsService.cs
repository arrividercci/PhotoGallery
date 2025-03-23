using WebServer.Application.Models;
using WebServer.Domain.Enums;

namespace WebServer.Application.Interfaces
{
    public interface IReactionsService
    {
        Task<ReactsModel> GetImageReactsAsync(int id);
        Task<ReactsModel> SetImageReactAsync(int id, string userId, ReactionType type);
    }
}
