using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerEngine.Core.Services
{
    using ServerEngine.Core.Services.Interfaces;

    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly ILogger<NewtonsoftJsonSerializer> _logger;
        private readonly StringEnumConverter _jsonConverter;

        public NewtonsoftJsonSerializer(ILogger<NewtonsoftJsonSerializer> logger)
        {
            _logger = logger;
            _jsonConverter = new StringEnumConverter();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation("CustomJsonSerializer initialize.");
        }

        /// <summary>
        /// Serialize.
        /// option 1: Convert enum to string.
        /// option 2: Serialize as Indent.
        /// </summary>
        public string Serialize(object dataObj)
        {
            return JsonConvert.SerializeObject(dataObj, Formatting.Indented, _jsonConverter);
        }

        /// <summary>
        /// Deserialize.
        /// option 1: Convert enum to string.
        /// </summary>
        public T Deserialize<T>(string dataString) where T : class
        {
            return JsonConvert.DeserializeObject<T>(dataString, _jsonConverter);
        }
    }
}
