using Microsoft.Extensions.Logging;
using WebServer.Application.Interfaces;
using WebServer.Application.Models;
using WebServer.Domain.Entities;
using WebServer.Domain.Enums;
using WebServer.Infrastructure.Interfaces;

namespace WebServer.Application.Services
{
    public class ReactionsService(IImageRepository imageRepository, ILogger<ReactionsService> logger) : IReactionsService
    {
        public async Task<ReactsModel> GetImageReactsAsync(int id)
        {
            logger.LogInformation("Fetching reactions for image with Id={ImageId}", id);

            var image = await imageRepository.GetImageWithReactsAsync(id);
            if (image == null)
            {
                logger.LogWarning("Image with Id={ImageId} not found", id);
                throw new ArgumentNullException($"Image with Id={id} not found.");
            }

            var likesCount = image.Reactions.Count(r => r.ReactionType == ReactionType.Like);
            var dislikeCount = image.Reactions.Count - likesCount;

            logger.LogInformation("Image Id={ImageId} has {LikesCount} likes and {DislikesCount} dislikes",
                id, likesCount, dislikeCount);

            return new ReactsModel
            {
                LikesCount = likesCount,
                DislikesCount = dislikeCount
            };
        }

        public async Task<ReactsModel> SetImageReactAsync(int id, string userId, ReactionType type)
        {
            logger.LogInformation("Setting reaction for ImageId={ImageId} by UserId={UserId} with Type={ReactionType}",
                id, userId, type);

            var image = await imageRepository.GetImageWithReactsAsync(id);
            if (image == null)
            {
                logger.LogWarning("Image with Id={ImageId} not found", id);
                throw new ArgumentNullException($"Image with Id={id} not found.");
            }

            var existingReaction = image.Reactions.FirstOrDefault(r => r.UserId == userId);

            if (existingReaction != null && existingReaction.ReactionType == type)
            {
                image.Reactions.Remove(existingReaction);
                logger.LogInformation("Removed existing reaction for ImageId={ImageId} by UserId={UserId}", id, userId);
            }
            else
            {
                if (existingReaction != null)
                {
                    image.Reactions.Remove(existingReaction);
                    logger.LogInformation("Updated reaction for ImageId={ImageId} by UserId={UserId} to Type={ReactionType}",
                        id, userId, type);
                }

                var newReaction = new UserReaction
                {
                    ImageId = id,
                    UserId = userId,
                    ReactionType = type
                };

                image.Reactions.Add(newReaction);
                logger.LogInformation("Added new reaction for ImageId={ImageId} by UserId={UserId} with Type={ReactionType}",
                    id, userId, type);
            }

            await imageRepository.SaveChangesAsync();

            var likesCount = image.Reactions.Count(r => r.ReactionType == ReactionType.Like);
            var dislikeCount = image.Reactions.Count - likesCount;

            logger.LogInformation("Updated reactions for ImageId={ImageId}: {LikesCount} likes, {DislikesCount} dislikes",
                id, likesCount, dislikeCount);

            return new ReactsModel
            {
                LikesCount = likesCount,
                DislikesCount = dislikeCount
            };
        }
    }

}
