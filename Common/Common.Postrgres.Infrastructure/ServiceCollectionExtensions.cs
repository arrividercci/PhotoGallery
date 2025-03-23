using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Common.Postrgres.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresDbContext<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            services.AddDbContext<TContext>((serviceProvider, dbContextOptionsBuilder) =>
            {
                dbContextOptionsBuilder.UseNpgsql(serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString(connectionString),
                    npgsqlOptionsBuilder => npgsqlOptionsBuilder.MigrationsAssembly(typeof(TContext).Assembly.FullName));
            });

            return services;
        }
    }
}
