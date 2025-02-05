
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataDesignerServer.Core.Services
{
    using DataDesignerServer.Core.Services.Interfaces;

    /// <summary>
    /// NewtonsoftJsonSerializer.
    /// </summary>
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public NewtonsoftJsonSerializer()
        {
            _serializerOptions = new JsonSerializerOptions();
            _serializerOptions.Converters.Add(new JsonStringEnumConverter());
            _serializerOptions.WriteIndented = true;
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        public string Serialize<T>(T data)
        {
            return JsonSerializer.Serialize(data, _serializerOptions);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _serializerOptions);
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public object Deserialize(string json, Type type)
        {
            return JsonSerializer.Deserialize(json, type, _serializerOptions);
        }
    }
}
