using Microsoft.AspNetCore.Identity;

namespace WebServer.Domain.Entities
{
    /// <summary>
    /// Represents a user who can have multiple albums.
    /// </summary>
    public class User : IdentityUser 
    {
        /// <summary>
        /// Gets or sets the list of albums owned by the user.
        /// </summary>
        public List<Album>? Albums { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }
        public List<UserReaction> Reactions { get; set; } = default!;
    }
}
