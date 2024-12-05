using DataDesigner.Core.Parser;
using DataDesigner.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;

namespace DataDesigner
{

    /// <summary>
    /// Program.
    /// </summary>
    class Program
    {
        public static void Main(string[] args)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            // Set configuration file.
            var configRoot = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddEnvironmentVariables()
                .Build();

            // Setup host.
            var host = Host.CreateDefaultBuilder()
                .ConfigureHostConfiguration(options =>
                {
                    // Configure host.
                    options.AddConfiguration(configRoot);
                    options.AddEnvironmentVariables();
                })
                .ConfigureLogging(options =>
                {
                    // Configure Logging.
                    options.ClearProviders();
                    options.AddNLog();
                    options.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .Build();

            var logger = LogManager
                .Setup()
                .SetupExtensions(ext => ext.RegisterConfigSettings(configRoot))
                .GetCurrentClassLogger();

            TestParse(host.Services);
        }

        private static void TestParse(IServiceProvider serviceProvider)
        {
            ////////////////////////////////////////////////////////////////////
            /// Setup test datas.
            var testCount = 3;
            var testClasses = new List<TestClass>();

            for (var cnt = 0; cnt < testCount; cnt++)
            {
                testClasses.Add(new TestClass
                {
                    TestInt = cnt,
                    TestFloat = cnt,
                    TestDouble = cnt,
                    TestString = cnt.ToString()
                });
            }

            // Json test data.
            var testClassJson = JsonConvert.SerializeObject(testClasses);
            var testEnums = JsonConvert.SerializeObject(Enum.GetValues<Type_Test>());

            ////////////////////////////////////////////////////////////////////
            /// Parser.
            var enumParser = new EnumParser(serviceProvider);
            enumParser.Parse(typeof(Type_Test).Name, testEnums);

        }
    }
}