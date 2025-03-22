using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebServer.Application.Helpers;
using WebServer.Application.Interfaces;
using WebServer.Application.Models;
using WebServer.Domain.Entities;

namespace WebServer.Application.Services
{
    public class JwtTokenService(UserManager<User> userManager, AuthConfig authConfig, ILogger<JwtTokenService> logger) : IJwtTokenService
    {
        public async Task<Token> CreateTokenAsync(User user, bool populateExpire)
        {
            logger.LogInformation("Start creating JWT token for user with email={Email}", user.Email);
            SigningCredentials signingCredentials = GetSigningCredentials();
            List<Claim> claims = await GetClaimsAsync(user);
            JwtSecurityToken tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            string refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;

            if (populateExpire)
            {
                user.RefreshTokenExpireTime = DateTime.UtcNow.AddHours(authConfig.RefreshTokenSettings.ExpiresHours);
            }

            await userManager.UpdateAsync(user);
            string? accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            Token token = new() { AccessToken = accessToken, RefreshToken = refreshToken, ExpirationTime = authConfig.JWTsettings.ExpiresSeconds };

            return token;
        }

        public async Task<Token> RefreshTokenAsync(TokenModel token)
        {
            logger.LogInformation("Start refreshing token");
            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token.AccessToken!);
            User? user = await userManager.FindByIdAsync(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (user == null
                || user.RefreshToken != token.RefreshToken
                || user.RefreshTokenExpireTime <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return await CreateTokenAsync(user, false);
        }

        private SigningCredentials GetSigningCredentials()
        {
            logger.LogInformation("Start getting signin credentials");
            byte[] key = Encoding.UTF8.GetBytes(authConfig.JWTsettings.Secret);
            SymmetricSecurityKey secret = new(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaimsAsync(User user)
        {
            logger.LogInformation("Start getting claims for token");
            List<Claim> claims = new() { new Claim(ClaimTypes.NameIdentifier, user.Id) };
            IList<string> roles = await userManager.GetRolesAsync(user);

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            logger.LogInformation("Start generating token options");
            JwtSecurityToken tokenOptions = new(authConfig.JWTsettings.Issuer,
                                                 authConfig.JWTsettings.Audience,
                                                 claims,
                                                 expires: DateTime.UtcNow.AddSeconds(authConfig.JWTsettings.ExpiresSeconds),
                                                 signingCredentials: signingCredentials);

            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            logger.LogInformation("Start generating refresh token");
            byte[] randomNumber = new byte[authConfig.RefreshTokenSettings.BytesNumber];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            logger.LogInformation("Getting pricipals from expired token={Token}", token);
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.JWTsettings.Secret)),
                ValidateLifetime = true,
                ValidIssuer = authConfig.JWTsettings.Issuer,
                ValidAudience = authConfig.JWTsettings.Audience,
            };

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken securityToken;
            ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
