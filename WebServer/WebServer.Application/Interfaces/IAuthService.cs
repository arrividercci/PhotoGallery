using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebServer.Application.Models;
using WebServer.Domain.Entities;

namespace WebServer.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginRequest loginRequest);
        Task<AuthResult> RegisterAsync(RegisterRequest registerRequest);
    }
}
