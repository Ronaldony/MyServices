using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataDesigner.Core.Parser
{
    /// <summary>
    /// EnumParser.
    /// </summary>
    internal sealed class EnumParser : IParser
    {
        private readonly ILogger<EnumParser> _logger;

        public EnumParser(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumParser>>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation($"EnumParser initialized.");
        }

        /// <summary>
        /// Parse enum type.
        /// </summary>
        public Dictionary<Type, List<Type>> Parse(string typeName, string typeValue)
        {
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[EnumParser] Parse datas.");

            try
            {
                var typeDict = new Dictionary<Type, List<Type>>();
                var lines = typeValue.Split(Environment.NewLine);

                foreach (var line in lines)
                {
                    var values = line.Split(',');
                    if (values.Length != 2)
                    {
                        _logger.LogError($"[EnumParser] Invalid line: {line}");
                        continue;
                    }

                    var key = values[0].Trim();
                    var value = values[1].Trim();

                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                    {
                        _logger.LogError($"[EnumParser] Invalid line: {line}");
                        continue;
                    }

                    //if (!typeDict.ContainsKey(key))
                    //{
                    //    typeDict.Add(key, new List<Type>());
                    //}

                    //typeDict[key].Add(value);
                }
                return typeDict;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

            }

            return default;
        }
    }
}
