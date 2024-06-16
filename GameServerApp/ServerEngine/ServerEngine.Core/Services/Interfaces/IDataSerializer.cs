namespace ServerEngine.Core.Services.Interfaces
{
    public interface IDataSerializer
    {
        /// <summary>
        /// Serialize data.
        /// </summary>
        byte[] Serialize<T>(T data) where T: class;

        /// <summary>
        /// Deserialize data object.
        /// </summary>
        T Deserialize<T>(byte[] dataObj) where T: class;
    }
}
