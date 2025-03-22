using WebServer.Domain.Abstraction;

namespace WebServer.Domain.Entities
{
    /// <summary>
    ///     Represents an album that contains a list of images and information about the author.
    /// </summary>
    public class Album : Entity<int>
    {
        /// <summary>
        ///     Gets or sets the name of the album.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        ///     Gets or sets the list of images belonging to this album.
        /// </summary>
        public List<Image> Images { get; set; } = default!;

        /// <summary>
        ///     Gets or sets the author of the album.
        /// </summary>
        public User? Author { get; set; }

        /// <summary>
        ///     Gets or sets the identifier of the user (author) who created this album.
        /// </summary>
        public required string AuthorId { get; set; }
    }
}
