using WebServer.Domain.Enums;

namespace WebServer.Domain.Entities
{
    public class UserReaction
    {
        public int ImageId {  get; set; }
        public Image? Image { get; set; }
        public string UserId {  get; set; }
        public User User {  get; set; }
        public ReactionType ReactionType { get; set; }
    }
}
