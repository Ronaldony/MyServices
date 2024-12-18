using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DataDesigner.Core.CodeGenerator
{
    /// <summary>
    /// EnumCodeGenerator.
    /// </summary>
    internal sealed class EnumCodeGenerator
    {
        private readonly ILogger<EnumCodeGenerator> _logger;

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
            _dllFilePath = Path.Combine(_dirPath, dllFileName);

            // Create compilation.
            _compilation = CSharpCompilation.Create(
                assemblyName: dllFileName,
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        /// <summary>
        /// Get generated file full path.
        /// </summary>
        public IEnumerable<Type> GetGeneratedTypes()
        {
            var assembly = Assembly.LoadFrom(_dllFilePath);

            return assembly.GetTypes();
        }

        /// <summary>
        /// Parse class code data, then generate enum code.
        /// </summary>
        /// <param name="dirPath">Directory path which file will be generated</param>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="enumMembers">EnumCodeData list</param>
        public void AddType(string enumNamespace, string enumName, IEnumerable<EnumMember> enumMembers)
        {
            _logger.LogDebug($"//////////////////////////////////////////////////////////");
            _logger.LogDebug($"[EnumCodeGenerator] Generating start");

            // Get compile unit.
            var compilationSyntax = CreateCompilationSyntax(enumNamespace, enumName, enumMembers);

            // Generate DLL.
            GenerateDLL(compilationSyntax);
            
            // Accumulate CompilationUnitSyntax.
            _compilationSyntaxs.Add($"{enumName}.cs", compilationSyntax);

            _logger.LogDebug($"[EnumCodeGenerator] Code generated");
        }

        /// <summary>
        /// Create files.
        /// </summary>
        /// <param name="dirPath"></param>
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
        /// Get MetadataReference.
        /// </summary>
        public MetadataReference GetMetadataReference()
        {
            return MetadataReference.CreateFromFile(_dllFilePath);
        }

        /// <summary>
        /// Write the file contains enum.
        /// </summary>
        private bool GenerateDLL(CompilationUnitSyntax compilationSyntax)
        {
            _compilation = _compilation.AddSyntaxTrees(compilationSyntax.SyntaxTree);

            // emit compilation to memory stream.
            var result = _compilation.Emit(_dllFilePath);
            
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
        /// <param name="enumMembers">EnumCodeData list</param>
        private CompilationUnitSyntax CreateCompilationSyntax(string enumNamespace, string enumName, IEnumerable<EnumMember> enumMembers)
        {
            // Create namespace.
            var declareNamespace = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.IdentifierName(enumNamespace));

            // Create enum declare.
            var declareEnum = SyntaxFactory
                .EnumDeclaration(enumName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            // Create enum member.
            foreach (var enumMember in enumMembers)
            {
                var memberSyntax = SyntaxFactory
                    .EnumMemberDeclaration(enumMember.Name) // Name
                    .WithEqualsValue(                       // Value
                        SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(enumMember.Value)
                                )
                            )
                        )
                .WithLeadingTrivia( // 멤버 주석 추가
                    SyntaxFactory.Comment($"// {enumMember.Comment}"),
                    SyntaxFactory.CarriageReturnLineFeed);

                // Add member.
                declareEnum = declareEnum.AddMembers(memberSyntax);
            }

            // Add member.
            declareNamespace = declareNamespace.AddMembers(declareEnum);

            return SyntaxFactory.CompilationUnit().AddMembers(declareNamespace).NormalizeWhitespace();
        }
    }
}
