namespace ServerEngine.Core.Services.Interfaces
{
    public interface IJsonSerializer
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize();

        /// <summary>
        /// Serialize object to json string.
        /// </summary>
        public string Serialize(object datgaObj);

        /// <summary>
        /// Deserialize json string to object.
        /// </summary>
        public T Deserialize<T>(string dataString) where T : class;
    }
}
