namespace GameCore.Interfaces
{
    /// <summary>
    /// ITDBService.
    /// </summary>
    public interface ITDBService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        bool Initialize();

        /// <summary>
        /// Get.
        /// </summary>
        T GetById<T>(int id) where T : class, ITableBase, new();

        /// <summary>
        /// First.
        /// </summary>
        T First<T>(Func<T, bool> predicate) where T : class, ITableBase, new();

        /// <summary>
        /// Last.
        /// </summary>
        T Last<T>(Func<T, bool> predicate) where T : class, ITableBase, new();

        /// <summary>
        /// Select.
        /// </summary>
        IEnumerable<T> Select<T>(Func<T, bool> predicate) where T : class, ITableBase, new();

        /// <summary>
        /// SelectAll.
        /// </summary>
        IEnumerable<T> SelectAll<T>() where T : class, ITableBase, new();

        /// <summary>
        /// Count.
        /// </summary>
        int Count<T>() where T : class, ITableBase, new();
    }
}
