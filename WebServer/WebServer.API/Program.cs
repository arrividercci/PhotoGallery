using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebServer.Infrastructure;
using WebServer.API.Extensions;
using WebServer.Domain.Entities;
using WebServer.Infrastructure.Interfaces;
using WebServer.Infrastructure.Repositories;
using WebServer.Application.Interfaces;
using WebServer.Application.Services;
using WebServer.Application.Helpers;
using System.Configuration;
using Storage.Abstraction;
using Storage.Azure;
using Microsoft.Extensions.Azure;
using WebServer.API.Middlewares;
using WebServer.API.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

//builder.Services.AddRepositories<PhotoGalleryDbContext>();

builder.Services.AddTransient<IAlbumRepository, AlbumRepository>();

builder.Services.AddTransient<IImageRepository, ImageRepository>();

builder.Services.AddPostgresDbContext<PhotoGalleryDbContext>("photoGallery");

builder.Services.AddIdentity<User, IdentityRole>(options => 
{
    IConfigurationSection userSettings = builder.Configuration.GetSection("Auth:UserSettings");
    options.SignIn.RequireConfirmedAccount = bool.Parse(userSettings["RequireConfirmedAccount"]!);
    options.User.AllowedUserNameCharacters = userSettings["AllowedUserNameCharacters"]!;
    options.User.RequireUniqueEmail = bool.Parse(userSettings["RequireUniqueEmail"]!);
}).AddEntityFrameworkStores<PhotoGalleryDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        IConfigurationSection jwtSettings = builder.Configuration.GetSection("Auth:JWTsettings");
        string secretKey = jwtSettings["Secret"]!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddSingleton(builder.Configuration.GetSection("Auth").Get<AuthConfig>() ??
    throw new ConfigurationErrorsException("Auth settings could not be loaded from configuration."));

builder.Services.AddSingleton(builder.Configuration.GetSection("Azure:Storage").Get<AzureBlobConfig>() ??
    throw new ConfigurationErrorsException("Azure blob settings could not be loaded from configuration."));

builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddTransient<IAlbumService, AlbumService>();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration.GetSection("Azure:Storage:ConnectionString").Value);
});

builder.Services.AddTransient<IStorageService, AzureBlobStorageService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "CorsPolicy",
        policyBuilder =>
        policyBuilder.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins(builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? []));
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    app.UseSwaggerUI();
    
    using IServiceScope scope = app.Services.CreateScope();
    
    var dbContext = scope.ServiceProvider.GetRequiredService<PhotoGalleryDbContext>();
    
    await dbContext.Database.MigrateAsync();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
