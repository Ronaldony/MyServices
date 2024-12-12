using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Runtime.Loader;

namespace DataDesigner.Core.CodeGenerator
{
    /// <summary>
    /// EnumCodeGenerator.
    /// </summary>
    internal sealed class EnumCodeGenerator
    {
        private readonly ILogger<EnumCodeGenerator> _logger;

        private readonly string _dllFileName;

        private List<Type> _generatedTypes;

        public EnumCodeGenerator(IServiceProvider serviceProvider, string dllFileName)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumCodeGenerator>>();

            _dllFileName = dllFileName;

            _generatedTypes = new List<Type>();
        }

        public void Initialize()
        {
            // Initialize.
        }

        /// <summary>
        /// Parse class code data, then generate enum code.
        /// </summary>
        /// <param name="dirPath">Directory path which file will be generated</param>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="codeDatas">EnumCodeData list</param>
        public void Generate(string dirPath, string enumNamespace, string enumName, IEnumerable<EnumCodeData> codeDatas)
        {
            _logger.LogInformation($"//////////////////////////////////////////////////////////");
            _logger.LogInformation($"[EnumCodeGenerator] Generating start...");

            // Get compile unit.
            var compileUnit = GetCompileUnit(enumNamespace, enumName, codeDatas);

            // Generate code.
            GenerateCode(compileUnit, dirPath, $"{enumName}.cs");

            _logger.LogInformation($"[EnumCodeGenerator] Code generated...");
        }

        /// <summary>
        /// Get assembly.
        /// </summary>
        public string GetDllFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dllFileName);
        }

        /// <summary>
        /// Get generated assembly.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetGeneratedTypes()
        {
            return _generatedTypes;
        }
        
        /// <summary>
        /// Set Assembly.
        /// </summary>
        private void EmitToDll(CSharpCompilation compilation)
        {
            // emit compilation to memory stream.
            var result = compilation.Emit(GetDllFilePath());

            if (result.Success)
            {
                var assembly = Assembly.LoadFrom(GetDllFilePath());
                _generatedTypes = assembly.GetTypes().ToList();
            }
            else
            {
                // fail.
                var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);

                foreach (var fail in failures)
                {
                    _logger.LogError($"enum code compilation failed. msg: {fail.ToString()}");
                }
            }
        }

        /// <summary>
        /// Get code compile unit for type.
        /// </summary>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="codeDatas">EnumCodeData list</param>
        private CodeCompileUnit GetCompileUnit(string enumNamespace, string enumName, IEnumerable<EnumCodeData> codeDatas)
        {
            // Code namespace.
            var codeNameSpace = new CodeNamespace(enumNamespace);

            // Declare enum type.
            var codeType = new CodeTypeDeclaration
            {
                Name = enumName,
                IsEnum = true,
                TypeAttributes = TypeAttributes.Public,
            };

            // Add code member.
            foreach (var codeData in codeDatas)
            {
                var codeMember = new CodeMemberField(enumName, codeData.Key);

                codeMember.InitExpression = new CodePrimitiveExpression(codeData.Value);
                codeMember.Comments.Add(new CodeCommentStatement
                {
                    Comment = new CodeComment(codeData.Comment)
                });

                codeType.Members.Add(codeMember);
            }

            // Add type into namespace.
            codeNameSpace.Types.Add(codeType);

            // Create CompileUnit.
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(codeNameSpace);

            return compileUnit;
        }

        /// <summary>
        /// Generate code
        /// </summary>
        /// <param name="compileUnit">Code compile unit/param>
        /// <param name="dirPath">Directory path which file will be generated</param>
        /// <param name="outputfileName">Output file name</param>
        private void GenerateCode(CodeCompileUnit compileUnit, string dirPath, string outputfileName)
        {
            using var codeProvider = new CSharpCodeProvider();

            // Create a TextWriter to a StreamWriter to the output file.
            using (StreamWriter sw = new StreamWriter(outputfileName, false))
            {
                using var tw = new IndentedTextWriter(sw, "    ");

                var options = new CodeGeneratorOptions();
                options.BlankLinesBetweenMembers = true;

                // Generate source code using the code provider.
                codeProvider.GenerateCodeFromCompileUnit(compileUnit, tw, options);

                // Close the output file.
                tw.Close();

                // Add syntax tree into compilation.
                var text = File.ReadAllText($"{dirPath}/{outputfileName}");
                var syntaxTree = CSharpSyntaxTree.ParseText(text);

                // Compilation.
                var compilation = CSharpCompilation.Create(
                    assemblyName: _dllFileName,
                    syntaxTrees: new [] { syntaxTree },
                    references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                EmitToDll(compilation);
            }
        }
    }
}
