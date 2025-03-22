namespace WebServer.Application.Helpers
{
    public class AuthConfig
    {
        public required JWTConfig JWTsettings { get; set; }
        public required RefreshTokenConfig RefreshTokenSettings { get; set; }

        public class RefreshTokenConfig
        {
            public required int ExpiresHours { get; set; }
            public required int BytesNumber { get; set; }
        }

        public class JWTConfig
        {
            public required string Issuer { get; set; }
            public required string Audience { get; set; }
            public required string Secret { get; set; }
            public required int ExpiresSeconds { get; set; }
        }
    }
}
