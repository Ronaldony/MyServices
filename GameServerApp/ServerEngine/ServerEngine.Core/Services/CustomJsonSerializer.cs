using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerEngine.Core.Services
{
    using ServerEngine.Core.Services.Interfaces;

    public sealed class CustomJsonSerializer : IJsonSerializer
    {
        private readonly ILogger<CustomJsonSerializer> _logger;
        private readonly StringEnumConverter _jsonConverter;

        public CustomJsonSerializer(ILogger<CustomJsonSerializer> logger)
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

        public string Serialize(object dataObj)
        {
            return JsonConvert.SerializeObject(dataObj, _jsonConverter);
        }

        public T Deserialize<T>(string dataString) where T : class
        {
            return JsonConvert.DeserializeObject<T>(dataString, _jsonConverter);
        }
    }
}
