namespace DataDesignerServer
{
    using DataDesignerServer.Core.Generator;
    using DataDesignerServer.Core.TypeManager.Class;
    using DataDesignerServer.Core.TypeManager.Enum;

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
            enumManager.Initialize(Directory.GetCurrentDirectory());

            // ClassManager.
            var classManager = _serviceProvider.GetRequiredService<ClassManager>();
            classManager.Initialize(Directory.GetCurrentDirectory());
        }
    }
}