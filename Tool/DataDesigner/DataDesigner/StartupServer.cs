

namespace DataDesigner
{
    using DataDesigner.Core.Generator;
    using DataDesigner.Core.TypeManager;
    using Microsoft.Extensions.DependencyInjection;

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
            var codeGenerator = _serviceProvider.GetRequiredService<CodeGenerator>();
            codeGenerator.Initialize();

            // EnumManager.
            var enumManager = _serviceProvider.GetRequiredService<EnumManager>();
            enumManager.Initialize(AppDomain.CurrentDomain.BaseDirectory);

            // ClassManager.
            var classManager = _serviceProvider.GetRequiredService<ClassManager>();
            classManager.Initialize(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
