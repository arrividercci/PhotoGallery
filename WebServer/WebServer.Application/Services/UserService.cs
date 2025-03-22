using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WebServer.Application.Interfaces;
using WebServer.Domain.Entities;

namespace WebServer.Application.Services
{
    public class UserService(UserManager<User> userManager, ILogger<AuthService> logger) : IUserService
    {
        public async Task<User?> GetByIdAsync(string id)
        {
            logger.LogInformation("Fetching user by ID: {UserId}", id);
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                logger.LogWarning("User with ID {UserId} not found", id);
            }
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            logger.LogInformation("Fetching user by email: {Email}", email);
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                logger.LogWarning("User with email {Email} not found", email);
            }
            return user;
        }

        public async Task<User?> GetBy(ClaimsPrincipal user)
        {
            logger.LogInformation("Fetching user by ClaimsPrincipal");
            return await userManager.GetUserAsync(user);
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            logger.LogInformation("Checking if user {UserId} is in role {Role}", user.Id, role);
            return await userManager.IsInRoleAsync(user, role);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            logger.LogInformation("Adding user {UserId} to role {Role}", user.Id, role);
            var result = await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to add user {UserId} to role {Role}", user.Id, role);
            }

            return result;
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
        {
            logger.LogInformation("Removing user {UserId} from role {Role}", user.Id, role);
            var result = await userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to remove user {UserId} from role {Role}", user.Id, role);
            }

            return result;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            logger.LogInformation("Fetching all users");
            return await userManager.Users.ToListAsync();
        }
    }
}
