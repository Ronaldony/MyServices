namespace ServerEngine.Database.Interfaces
{
    /// <summary>
    /// IDBObjectConn.
    /// </summary>
    public interface IDBObjectConn
    {
        Task InitializeAsync();

        T GetConnection<T>() where T : class;
    }
}
