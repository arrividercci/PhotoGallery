namespace WebServer.Application.Models
{
    public class AlbumModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AuthorId {  get; set; }
        public List<ImageModel> Images { get; set; } = default!;
    }
}
