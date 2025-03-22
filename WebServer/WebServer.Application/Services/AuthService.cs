using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WebServer.Application.Interfaces;
using WebServer.Application.Models;
using WebServer.Domain.Entities;

namespace WebServer.Application.Services
{
    public class AuthService(UserManager<User> userManager, ILogger<AuthService> logger) : IAuthService
    {
        public async Task<AuthResult> RegisterAsync(RegisterRequest registerRequest)
        {
            logger.LogInformation("Registering new user: {Username}", registerRequest.Username);

            var user = new User
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email
            };

            var result = await userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                logger.LogWarning("User registration failed for {Username}: {Errors}",
                    registerRequest.Username, string.Join(", ", result.Errors.Select(e => e.Description)));

                return new AuthResult { Succeeded = false, Errors = result.Errors };
            }

            logger.LogInformation("User {Username} registered successfully", registerRequest.Username);
            return new AuthResult { Succeeded = true, User = user };
        }

        public async Task<AuthResult> LoginAsync(LoginRequest loginRequest)
        {
            logger.LogInformation("User login attempt: {Username}", loginRequest.Username);

            var user = await userManager.FindByNameAsync(loginRequest.Username);

            if (user == null)
            {
                logger.LogWarning("Login failed: User {Username} not found", loginRequest.Username);
                return new AuthResult { Succeeded = false };
            }

            var passwordCheck = await userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!passwordCheck)
            {
                logger.LogWarning("Login failed: Incorrect password for user {Username}", loginRequest.Username);
                return new AuthResult { Succeeded = false };
            }

            logger.LogInformation("User {Username} logged in successfully", loginRequest.Username);
            return new AuthResult { Succeeded = true, User = user };
        }
    }

}
