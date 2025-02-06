
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataDesignerServer.Core.Services
{
    using DataDesignerServer.Core.Services.Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// NewtonsoftJsonSerializer.
    /// </summary>
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public NewtonsoftJsonSerializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter()},
                Formatting = Formatting.Indented,
            };
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        public string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data, _jsonSerializerSettings);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, _jsonSerializerSettings);
        }
    }
}
