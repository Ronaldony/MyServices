using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
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

        public EnumCodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumCodeGenerator>>();

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
            var codeProvider = new CSharpCodeProvider();

            // Create a TextWriter to a StreamWriter to the output file.
            using (StreamWriter sw = new StreamWriter(outputfileName, false))
            {
                var tw = new IndentedTextWriter(sw, "    ");

                var options = new CodeGeneratorOptions();
                options.BlankLinesBetweenMembers = true;

                // Generate source code using the code provider.
                codeProvider.GenerateCodeFromCompileUnit(compileUnit, tw, options);

                // Close the output file.
                tw.Close();

                // Complie.
                var syntasTree = CSharpSyntaxTree.ParseText(File.ReadAllText($"{dirPath}/{outputfileName}"));

                var compilation = CSharpCompilation.Create(
                    assemblyName: "MyAssembly",
                    syntaxTrees: new[] { syntasTree },
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


                using (var ms = new MemoryStream())
                {
                    EmitResult result = compilation.Emit(ms);

                    if (result.Success)
                    {
                        ms.Seek(0, SeekOrigin.Begin);

                        var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                        var types = assembly.GetTypes();
                    }
                }
            }
        }
    }
}
