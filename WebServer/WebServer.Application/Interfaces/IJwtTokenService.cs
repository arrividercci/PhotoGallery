using WebServer.Application.Models;
using WebServer.Domain.Entities;

namespace WebServer.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<Token> CreateTokenAsync(User user, bool populateExpire);
        Task<Token> RefreshTokenAsync(TokenModel token);
    }
}
