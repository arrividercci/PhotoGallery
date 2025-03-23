using Microsoft.EntityFrameworkCore;

namespace Common.Data
{
    public static class DbContextExtensions
    {
        public static void ConfigureEntities<T>(this DbContext dbContext, ModelBuilder modelBuilder)
        {
            List<Type> types = typeof(T).Assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false, IsNested: false })
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>)))
                .ToList();

            foreach (Type type in types)
            {
                modelBuilder.Entity(type).ToTable(type.Name).HasKey(nameof(IEntity<int>.Id));
            }
        }
    }
}
