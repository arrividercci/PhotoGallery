using System.ComponentModel.DataAnnotations;

namespace WebServer.Application.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
