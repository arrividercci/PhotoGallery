namespace WebServer.Domain.Abstraction
{
    /// <summary>
    ///     Base class that represents Entities.
    /// </summary>
    /// <typeparam name="T">Type parameter of Id.</typeparam>
    public class Entity<T> : IEntity<T> where T : struct
    {
        /// <summary>
        ///     Gets or sets Id of the IEntity
        /// </summary>
        public T Id { get; set; }
    }
}
