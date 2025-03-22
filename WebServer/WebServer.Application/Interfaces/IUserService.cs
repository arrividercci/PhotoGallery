using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebServer.Domain.Entities;

namespace WebServer.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetBy(ClaimsPrincipal user);
        Task<bool> IsInRoleAsync(User user, string role);
        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IdentityResult> RemoveFromRoleAsync(User user, string role);
        Task<List<User>> GetAllUsersAsync();
    }
}
