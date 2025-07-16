namespace Framework.Core.Database.Interfaces
{
    /// <summary>
    /// IDataObject.
    /// </summary>
    public interface IDataObject
    {
        /// <summary>
        /// Load data async.
        /// </summary>
        Task LoadDataAsync(string pid);

        /// <summary>
        /// Upsert async.
        /// </summary>
        Task<bool> UpsertAsync(string pid);
    }
}
