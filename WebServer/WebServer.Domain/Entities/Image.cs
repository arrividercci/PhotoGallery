using Common.Data;

namespace WebServer.Domain.Entities
{
    /// <summary>
    ///     Represents an image that belongs to an album.
    /// </summary>
    public class Image : Entity<int>
    {
        /// <summary>
        ///     URL of the image.
        /// </summary>
        public required string Url { get; set; }
        public required string Key { get; set; }
        public required string ContentType { get; set; }
        public List<UserReaction> Reactions { get; set; } = default!;
    }
}
