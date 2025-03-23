using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Common.Data.Infrastracture
{
    public static class ServiceCollectionExtensions
    {
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
