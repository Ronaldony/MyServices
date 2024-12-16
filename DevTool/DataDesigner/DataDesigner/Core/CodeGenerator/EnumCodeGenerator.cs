using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;

namespace DataDesigner.Core.CodeGenerator
{
    /// <summary>
    /// EnumCodeGenerator.
    /// </summary>
    internal sealed class EnumCodeGenerator
    {
        private readonly ILogger<EnumCodeGenerator> _logger;

        private readonly string _fileName;
        private readonly string _ditPath;

        private CSharpCompilation _compilation;

        public EnumCodeGenerator(IServiceProvider serviceProvider, string dirPath, string dllFileName)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumCodeGenerator>>();

            _fileName = dllFileName;
            _ditPath = dirPath;

            _compilation = CSharpCompilation.Create(
                assemblyName: _fileName,
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        public void Initialize()
        {
            // Initialize.
            _logger.LogInformation("EnumCodeGenerator initialized.");
        }

        /// <summary>
        /// Get generated file full path.
        /// </summary>
        public string GetFilePath()
        {
            return Path.Combine(_ditPath, _fileName);
        }

        /// <summary>
        /// Get generated file full path.
        /// </summary>
        public IEnumerable<Type> GetNewTypes()
        {
            var assembly = Assembly.LoadFrom(GetFilePath());

            return assembly.GetTypes();
        }

        /// <summary>
        /// Parse class code data, then generate enum code.
        /// </summary>
        /// <param name="dirPath">Directory path which file will be generated</param>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="codeDatas">EnumCodeData list</param>
        public void AddType(string dirPath, string enumNamespace, string enumName, IEnumerable<EnumCodeData> codeDatas)
        {
            _logger.LogDebug($"//////////////////////////////////////////////////////////");
            _logger.LogDebug($"[EnumCodeGenerator] Generating start");

            // Get compile unit.
            var compileUnit = CreateCompileUnit(enumNamespace, enumName, codeDatas);

            // Generate code.
            GenerateCodeFile(compileUnit, $"{enumName}.cs");

            _logger.LogDebug($"[EnumCodeGenerator] Code generated");
        }
        
        /// <summary>
        /// Write the file contains enum.
        /// </summary>
        public bool CreateFile()
        {
            // emit compilation to memory stream.
            var result = _compilation.Emit(GetFilePath());

            if (!result.Success)
            {
                // Fail.
                var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);

                foreach (var fail in failures)
                {
                    _logger.LogError($"enum code compilation failed. msg: {fail.ToString()}");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Get code compile unit for type.
        /// </summary>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="codeDatas">EnumCodeData list</param>
        private CodeCompileUnit CreateCompileUnit(string enumNamespace, string enumName, IEnumerable<EnumCodeData> codeDatas)
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
            codeNameSpace.Comments.Clear();

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
        private void GenerateCodeFile(CodeCompileUnit compileUnit, string outputfileName)
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
                var fileText = File.ReadAllText($"{_ditPath}/{outputfileName}");
                var syntaxTree = CSharpSyntaxTree.ParseText(fileText);
                var root = syntaxTree.GetRoot();

                _compilation = _compilation.AddSyntaxTrees(syntaxTree);
            }
        }
    }
}
