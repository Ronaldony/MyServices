using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public string DllFilePath => _dllFilePath;

        private string _dllFileName;
        private string _dirPath;
        private string _dllFilePath;

        /// <summary>
        /// CompilationUnitSyntaxs.
        /// key: file name.
        /// value: CompilationUnitSyntax.
        /// </summary>
        private readonly Dictionary<string, CompilationUnitSyntax> _compilationSyntaxs;

        private CSharpCompilation _compilation;

        public EnumCodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<EnumCodeGenerator>>();

            _compilation = CSharpCompilation.Create(
                assemblyName: _dllFileName,
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            _compilationSyntaxs = new Dictionary<string, CompilationUnitSyntax>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="dirPath">Directory path to generate</param>
        /// <param name="dllFileName">DLL file name to generate</param>
        public void Initialize(string dirPath, string dllFileName)
        {
            // Initialize.
            _logger.LogInformation("EnumCodeGenerator initialized.");

            _dirPath = dirPath;
            _dllFileName = dllFileName;
            _dllFilePath = Path.Combine(_dirPath, dllFileName);
        }

        /// <summary>
        /// Get generated file full path.
        /// </summary>
        public IEnumerable<Type> GetGeneratedTypes()
        {
            var assembly = Assembly.LoadFrom(DllFilePath);

            return assembly.GetTypes();
        }

        /// <summary>
        /// Parse class code data, then generate enum code.
        /// </summary>
        /// <param name="dirPath">Directory path which file will be generated</param>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="codeDatas">EnumCodeData list</param>
        public void AddType(string enumNamespace, string enumName, IEnumerable<EnumCodeMember> codeDatas)
        {
            _logger.LogDebug($"//////////////////////////////////////////////////////////");
            _logger.LogDebug($"[EnumCodeGenerator] Generating start");

            // Get compile unit.
            var compilationSyntax = CreateCompilationSyntax(enumNamespace, enumName, codeDatas);

            // Generate DLL.
            GenerateDLL(compilationSyntax);
            
            // Accumulate CompilationUnitSyntax.
            _compilationSyntaxs.Add($"{enumName}.cs", compilationSyntax);

            _logger.LogDebug($"[EnumCodeGenerator] Code generated");
        }

        public void CreateFiles(string dirPath)
        {
            // Create file for generating types.
            foreach (var pair in _compilationSyntaxs)
            {
                var fileName = pair.Key;
                var compilationSyntax = pair.Value;

                var filePath = Path.Combine(_dirPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.WriteAllText(filePath, compilationSyntax.ToFullString());
            }
        }
        
        /// <summary>
        /// Write the file contains enum.
        /// </summary>
        private bool GenerateDLL(CompilationUnitSyntax compilationSyntax)
        {
            _compilation = _compilation.AddSyntaxTrees(compilationSyntax.SyntaxTree);

            // emit compilation to memory stream.
            var result = _compilation.Emit(DllFilePath);
            
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
        /// Create CodeCompileUnit\.
        /// </summary>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="codeMembers">EnumCodeData list</param>
        private CompilationUnitSyntax CreateCompilationSyntax(string enumNamespace, string enumName, IEnumerable<EnumCodeMember> codeMembers)
        {
            // Create namespace.
            var namespaceSyntax = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.IdentifierName(enumNamespace));

            // Create enum declare.
            var enumDeclare = SyntaxFactory.EnumDeclaration(enumName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            // Create enum member.
            foreach (var codeMember in codeMembers)
            {
                var enumMember = SyntaxFactory
                    .EnumMemberDeclaration(codeMember.Name) // Name
                    .WithEqualsValue(                       // Value
                        SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(codeMember.Value)
                                )
                            )
                        )
                .WithLeadingTrivia( // 멤버 주석 추가
                    SyntaxFactory.Comment($"// {codeMember.Comment}"),
                    SyntaxFactory.CarriageReturnLineFeed);

                // Add member.
                enumDeclare = enumDeclare.AddMembers(enumMember);
            }

            // Add member.
            namespaceSyntax = namespaceSyntax.AddMembers(enumDeclare);

            return SyntaxFactory.CompilationUnit().AddMembers(namespaceSyntax).NormalizeWhitespace();
        }
    }
}
