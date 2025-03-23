using Common.Data;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebServer.Infrastructure.Tests
{
    public class RepositoryTestBase
    {
        public RepositoryTestBase()
        {
            DbContext = CreateDbContext();
        }

        protected PhotoGalleryDbContext DbContext { get; }

        protected PhotoGalleryDbContext CreateDbContext()
        {
            DbContextOptionsBuilder<PhotoGalleryDbContext> builder =
                new DbContextOptionsBuilder<PhotoGalleryDbContext>().UseInMemoryDatabase($"CarsReviewDbContext-{Guid.NewGuid()}");

            return new PhotoGalleryDbContext(builder.Options);
        }

        protected TRepository CreateRepository<TRepository, TEntity, TKey>(Func<PhotoGalleryDbContext, ILogger<TRepository>, TRepository> func)
            where TRepository : IRepository<TEntity, TKey>
            where TEntity : Entity<TKey>
            where TKey : struct
        {
            ILogger<TRepository> logger = A.Fake<ILogger<TRepository>>();

            return func(DbContext, logger);
        }
    }
}
