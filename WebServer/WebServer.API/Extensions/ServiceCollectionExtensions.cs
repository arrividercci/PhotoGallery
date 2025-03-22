using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebServer.Infrastructure.Interfaces;
using WebServer.Infrastructure.Repositories;
namespace WebServer.API.Extensions
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

        /// <summary>
        ///     Registers all types that are assignable to the provided type as transient services.
        /// </summary>
        /// <param name="services">ServiceCollection to be registered at.</param>
        /// <param name="assembly">Source assembly where implementations should be found.</param>
        /// <typeparam name="T">Parent interface.</typeparam>
        public static void AddAllTransientFor<T>(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly(); // Use calling assembly if not provided

            var commandTypes = assembly.GetTypes()
                                       .Where(t => t.IsAssignableTo(typeof(T)) &&
                                                   t is { IsInterface: false, IsAbstract: false })
                                       .ToList();

            foreach (Type commandType in commandTypes)
            {
                services.AddTransient(commandType);
            }
        }

        public static void AddRepositories<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            Assembly assembly = typeof(TContext).Assembly;

            List<Type> repositoryTypes = assembly.GetTypes()
                                                 .Where(t => t.IsAssignableTo(typeof(IRepository)) && t is { IsInterface: false, IsAbstract: false })
                                                 .ToList();

            repositoryTypes.ForEach(repositoryType =>
            {
                List<Type> interfaceTypes = repositoryType.GetInterfaces().Where(i => i != typeof(IRepository)).ToList();
                // Register as implemented interfaces: so we can resolve both IRepository<Car> and ICarRepository
                interfaceTypes.ForEach(interfaceType => { services.AddTransient(interfaceType, repositoryType); });
            });

            // Register default repository
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
        }
    }
}
