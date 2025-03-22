using Microsoft.AspNetCore.Mvc;
using WebServer.Application.Interfaces;
using WebServer.Application.Models;

namespace WebServer.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService, IJwtTokenService jwtTokenService, ILogger<AuthController> logger) : ControllerBase
    {
        /// <summary>
        ///     Authenticates a user and returns an access token if successful.
        /// </summary>
        /// <param name="loginModel">User credentials (username and password).</param>
        /// <returns>Returns a JWT token if authentication is successful, otherwise Unauthorized.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<Token?>> Login([FromBody] LoginRequest loginModel)
        {
            logger.LogInformation("Login attempt for user: {Username}", loginModel.Username);

            var authResult = await authService.LoginAsync(loginModel);

            if (!authResult.Succeeded)
            {
                logger.LogWarning("Login failed for user: {Username}", loginModel.Username);
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            logger.LogInformation("Login successful for user: {Username}", loginModel.Username);

            var token = await jwtTokenService.CreateTokenAsync(authResult.User!, true);

            return Ok(token);
        }

        /// <summary>
        ///     Registers a new user and returns an access token if successful.
        /// </summary>
        /// <param name="registerModel">User registration details (username, email, password).</param>
        /// <returns>Returns a JWT token if registration is successful, otherwise BadRequest with errors.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<Token?>> Register([FromBody] RegisterRequest registerModel)
        {
            logger.LogInformation("Registration attempt for user: {Username}", registerModel.Username);

            var authResult = await authService.RegisterAsync(registerModel);

            if (!authResult.Succeeded)
            {
                logger.LogWarning("Registration failed for user: {Username}. Errors: {Errors}", registerModel.Username, string.Join(", ", authResult.Errors ?? []));

                return BadRequest(new { Message = "Registration failed.", authResult.Errors });
            }

            logger.LogInformation("Registration successful for user: {Username}", registerModel.Username);

            var token = await jwtTokenService.CreateTokenAsync(authResult.User!, true);

            return Ok(token);
        }

        /// <summary>
        ///     Refresh access token.
        /// </summary>
        /// <param name="token">Token to refresh.</param>
        /// <returns>Returns access, refresh tokens and expire time is seconds<see cref="TokenModel" />.</returns>
        [HttpPost("refresh")]
        public async Task<Token> Refresh([FromBody] TokenModel token)
        {
            Token tokenModel = await jwtTokenService.RefreshTokenAsync(token);

            return tokenModel;
        }

        /// <summary>
        ///     Gets data about user.
        /// </summary>
        /// <returns><see cref="UserModel" />.</returns>
        /*[HttpGet("me")]
        [Authorize]
        public async Task<UserModel?> GetUser()
        {
            string? userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            User? user = await authService.GetByIdAsync(userId!);

            return user.ToUserModel();
        }*/
    }
}
