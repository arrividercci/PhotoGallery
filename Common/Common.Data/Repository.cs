using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Common.Data
{
    public class Repository<TEntity, TKey>(DbContext dbContext, ILogger logger) : IRepository<TEntity, TKey>
        where TEntity : Entity<TKey> where TKey : struct
    {
        protected ILogger Logger => logger;

        protected DbSet<TEntity> AllData => dbContext.Set<TEntity>();
        /// <summary>
        ///     Retrieves an entity by its ID asynchronously.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        public async Task<TEntity?> GetByAsync(TKey id)
        {
            logger.LogDebug("Fetching entity {EntityType} with ID {EntityId} from database.", typeof(TEntity).Name, id);
            return await AllData.FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        /// <summary>
        ///     Adds an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public async Task AddAsync(TEntity entity)
        {
            logger.LogInformation("Adding a new entity of type {EntityType} to the database.", typeof(TEntity).Name);
            await AllData.AddAsync(entity);
        }

        /// <summary>
        ///     Adds an entity synchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            logger.LogInformation("Adding a new entity of type {EntityType} to the database.", typeof(TEntity).Name);
            AllData.Add(entity);
        }

        /// <summary>
        ///     Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            logger.LogInformation("Updating entity of type {EntityType} with ID {EntityId}.", typeof(TEntity).Name, entity.Id);
            AllData.Update(entity);
        }

        /// <summary>
        ///     Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(TEntity entity)
        {
            logger.LogWarning("Deleting entity of type {EntityType} with ID {EntityId}.", typeof(TEntity).Name, entity.Id);
            AllData.Remove(entity);
        }

        /// <summary>
        ///     Finds entities matching the specified expression.
        /// </summary>
        /// <param name="expression">The filter expression.</param>
        /// <returns>An IQueryable containing the matching entities.</returns>
        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> expression)
        {
            logger.LogDebug("Finding entities of type {EntityType} using expression {Expression}.", typeof(TEntity).Name, expression);
            return AllData.Where(expression);
        }

        /// <summary>
        ///     Saves changes to the database asynchronously.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            logger.LogDebug("Saving changes to the database.");
            await dbContext.SaveChangesAsync();
        }
    }
}
