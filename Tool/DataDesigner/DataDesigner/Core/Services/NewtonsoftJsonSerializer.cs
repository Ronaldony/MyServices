using Newtonsoft.Json;

namespace DataDesigner.Core.Services
{
    using DataDesigner.Core.Services.Interfaces;

    /// <summary>
    /// NewtonsoftJsonSerializer.
    /// </summary>
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// Deserialize.
        /// </summary>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        public string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }
    }
}
