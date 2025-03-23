using System.Linq.Expressions;

namespace Common.Data
{
    /// <summary>
    /// Base repository interface for generic data access.
    /// </summary>
    public interface IRepository
    {
    }

    /// <summary>
    ///     Repository interface for working with entities of type <typeparamref name="TEntity"/> with a key of type <typeparamref name="TKey"/>.
    ///     This interface provides common methods for data access operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity key.</typeparam>
    public interface IRepository<TEntity, TKey> : IRepository where TEntity : Entity<TKey> where TKey : struct
    {
        /// <summary>
        ///     Asynchronously retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity with the given identifier, or null if not found.</returns>
        public Task<TEntity?> GetByAsync(TKey id);

        /// <summary>
        ///     Asynchronously adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task AddAsync(TEntity entity);

        /// <summary>
        ///     Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public void Add(TEntity entity);

        /// <summary>
        ///     Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        public void Update(TEntity entity);

        /// <summary>
        ///     Deletes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public void Delete(TEntity entity);

        /// <summary>
        ///     Finds entities that match the given criteria.
        /// </summary>
        /// <param name="expression">A lambda expression defining the search criteria.</param>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that represents the entities matching the criteria.</returns>
        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        ///     Asynchronously saves changes made to the repository.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task SaveChangesAsync();
    }
}
