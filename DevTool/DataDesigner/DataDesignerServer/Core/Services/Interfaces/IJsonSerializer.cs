namespace DataDesignerServer.Core.Services.Interfaces
{
    /// <summary>
    /// interface for Json serilizer.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serialize data to json.
        /// </summary>
        public string Serialize<T>(T data);

        /// <summary>
        /// Deserialize json to data.
        /// </summary>
        public T Deserialize<T>(string json);

        /// <summary>
        /// Deserialize json to data.
        /// </summary>
        public object Deserialize(string json, Type type);
    }
}
