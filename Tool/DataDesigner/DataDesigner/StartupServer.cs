using DataDesigner.Core.DataManager;
using DataDesigner.Core.Generator;

namespace DataDesigner
{
    public class StartupServer
    {
        private readonly IServiceProvider _serviceProvider;

        public StartupServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Startup()
        {
            // CodeGenerator.
            var dataCodeGenerator = _serviceProvider.GetRequiredService<CodeGenerator>();
            dataCodeGenerator.Initialize();

            // EnumManager.
            var enumManager = _serviceProvider.GetRequiredService<EnumManager>();
            enumManager.Initialize(AppDomain.CurrentDomain.BaseDirectory);

            // ClassManager.
            var classManager = _serviceProvider.GetRequiredService<ClassManager>();
            classManager.Initialize(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
