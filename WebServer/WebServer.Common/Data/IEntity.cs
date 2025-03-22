namespace WebServer.Common.Data
{
    /// <summary>
    ///     IEntity interface.
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    ///     IEntity interface with generic type.
    /// </summary>
    /// <typeparam name="T">Id type.</typeparam>
    public interface IEntity<T> : IEntity
    {
        /// <summary>
        ///     Gets or sets Id of the IEntity
        /// </summary>
        public T Id { get; set; }
    }
}
