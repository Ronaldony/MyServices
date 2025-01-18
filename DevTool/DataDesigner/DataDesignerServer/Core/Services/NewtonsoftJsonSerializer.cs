using Newtonsoft.Json;

namespace DataDesignerServer.Core.Services
{
    using DataDesignerServer.Core.Services.Interfaces;

    /// <summary>
    /// NewtonsoftJsonSerializer.
    /// </summary>
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// Serialize.
        /// </summary>
        public string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
