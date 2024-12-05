using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CodeDom;

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
        /// <typeparam name="T">Parse value type</typeparam>
        /// <param name="typeName">Type name</param>
        /// <param name="typeValue">Type value</param>
        /// <returns>
        /// Type: Type of type name.
        /// List<Type>: Values of type value.
        /// </returns>
        public Dictionary<Type, List<Type>> Parse<T>(string typeName, T typeValue)
        {
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[EnumParser] Parse datas.");

            
            var codeType = new CodeTypeDeclaration(typeName);

            var start = new CodeEntryPointMethod();
            var cs1 = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("System.Console"),
                "WriteLine", new CodePrimitiveExpression("Hello World!"));
            start.Statements.Add(cs1);

            codeType.Members.Add(start);

            // Namespace.
            var codeNameSpace = new CodeNamespace("samples");

            codeNameSpace.Imports.Add(new CodeNamespaceImport("System"));
            codeNameSpace.Imports.Add(new CodeNamespaceImport("GameCore"));
            codeNameSpace.Types.Add(codeType);

            // CompileUnit.
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(codeNameSpace);

            return default;
        }
    }
}
