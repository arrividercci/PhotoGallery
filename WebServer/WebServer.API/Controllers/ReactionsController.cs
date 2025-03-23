using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebServer.Application.Interfaces;
using WebServer.Application.Models;
using WebServer.Domain.Enums;

namespace WebServer.API.Controllers
{
    [Route("api/reactions")]
    [ApiController]
    public class ReactionsController(IReactionsService reactionsService, ILogger<ReactionsController> logger) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<ReactsModel>> GetImageReactionsAsync(int id)
        {
            logger.LogInformation("Received request to get reactions for ImageId={ImageId}", id);

            try
            {
                var result = await reactionsService.GetImageReactsAsync(id);
                logger.LogInformation("Successfully retrieved reactions for ImageId={ImageId}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while fetching reactions for ImageId={ImageId}", id);
                return StatusCode(500, "An error occurred while retrieving reactions.");
            }
        }

        [HttpPost("{id}")]
        [Authorize]
        public async Task<ActionResult<ReactsModel>> SetImageReactionAsync(int id, [FromBody] ReactionType reactionType)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("Unauthorized attempt to set reaction for ImageId={ImageId}", id);
                return Unauthorized("User is not authenticated.");
            }

            logger.LogInformation("Received request to set reaction for ImageId={ImageId} by UserId={UserId} with Type={ReactionType}",
                id, userId, reactionType);

            try
            {
                var result = await reactionsService.SetImageReactAsync(id, userId, reactionType);
                logger.LogInformation("Successfully set reaction for ImageId={ImageId} by UserId={UserId} with Type={ReactionType}",
                    id, userId, reactionType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while setting reaction for ImageId={ImageId} by UserId={UserId}", id, userId);
                return StatusCode(500, "An error occurred while setting reaction.");
            }
        }
    }

}
