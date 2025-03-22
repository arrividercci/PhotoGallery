using Microsoft.AspNetCore.Identity;
using WebServer.Domain.Entities;

namespace WebServer.Application.Models
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public User? User { get; set; }
        public IEnumerable<IdentityError>? Errors { get; set; }
    }
}
