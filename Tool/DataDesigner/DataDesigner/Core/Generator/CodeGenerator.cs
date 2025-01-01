using DataDesigner.Core.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;

namespace DataDesigner.Core.Generator
{
    /// <summary>
    /// CodeGenerator.
    /// </summary>
    internal sealed class CodeGenerator
    {
        private readonly ILogger<CodeGenerator> _logger;

        /// <summary>
        /// Generated types.
        /// </summary>
        private List<Type> _generatedTypes;

        /// <summary>
        /// Type list.
        /// </summary>
        private List<Type> _typeList;

        /// <summary>
        /// CompilationUnitSyntax for class.
        /// key: file name.
        /// value: CompilationUnitSyntax.
        /// </summary>
        private readonly Dictionary<string, CompilationUnitSyntax> _classUnitSyntaxes;

        /// <summary>
        /// CompilationUnitSyntax for enum.
        /// key: file name.
        /// value: CompilationUnitSyntax.
        /// </summary>
        private readonly Dictionary<string, CompilationUnitSyntax> _enumUnitSyntaxes;

        /// <summary>
        /// Type table dictionary.
        /// key: type name.
        /// value: Type.
        /// </summary>
        private Dictionary<string, Type> _primitiveTypeDict;

        /// <summary>
        /// Compilation.
        /// </summary>
        private CSharpCompilation _compilation;

        public CodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CodeGenerator>>();

            _classUnitSyntaxes = new Dictionary<string, CompilationUnitSyntax>();
            _enumUnitSyntaxes = new Dictionary<string, CompilationUnitSyntax>();

            _primitiveTypeDict = new Dictionary<string, Type>();
            _typeList = new List<Type>();

            _generatedTypes = new List<Type>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="dirPath">Directory path to generate</param>
        /// <param name="dllFileName">DLL file name to generate</param>
        public void Initialize()
        {
            // Create compilation.
            var dllFile = typeof(CodeGenerator).Name + ".dll";

            _compilation = CSharpCompilation.Create(
                assemblyName: dllFile,
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Setup all types.
            _typeList.Clear();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = asm.GetTypes();

                _typeList.AddRange(types);
            }

            // Set up primitive types.
            _primitiveTypeDict.Clear();
            _primitiveTypeDict.Add("int", typeof(int));
            _primitiveTypeDict.Add("long", typeof(long));
            _primitiveTypeDict.Add("float", typeof(float));
            _primitiveTypeDict.Add("double", typeof(double));
            _primitiveTypeDict.Add("string", typeof(string));
            _primitiveTypeDict.Add("bool", typeof(bool));
            _primitiveTypeDict.Add("date", typeof(DateTime));

            // Initialize.
            _logger.LogInformation("EnumCodeGenerator initialized.");
        }

        /// <summary>
        /// Get new types from generated.
        /// </summary>
        public IEnumerable<Type> GetNewTypes()
        {
            return _generatedTypes;
        }

        /// <summary>
        /// Parse class code data, then generate enum code.
        /// </summary>
        public void AddEnum(string enumNamespace, string enumName, IEnumerable<EnumMember> enumMembers)
        {
            _logger.LogDebug($"//----------------------------------------------------");
            _logger.LogDebug($"[CodeGenerator] Generating enum {enumName}");

            // Get compile unit.
            var compilationSyntax = CreateEnumCompilationUnit(enumNamespace, enumName, enumMembers);

            // Accumulate CompilationUnitSyntax.
            _enumUnitSyntaxes.Add($"{enumName}.cs", compilationSyntax);

            // Refresh types.
            RefreshTypes(compilationSyntax);

            _logger.LogDebug($"[CodeGenerator] Enum {enumName} generated.");
        }

        /// <summary>
        /// Parse class code data, then generate class code.
        /// </summary>
        public bool AddClass(string classNamespace, string className, IEnumerable<ClassMember> classMembers)
        {
            _logger.LogDebug($"//----------------------------------------------------");
            _logger.LogDebug($"[CodeGenerator] Generating class {className}");

            // Get compile unit.
            var compilationSyntax = CreateClassCompilationUnit(classNamespace, className, classMembers);

            if (null == compilationSyntax)
            {
                _logger.LogError($"[CodeGenerator] Failed to generate {className}.");
                return false;
            }

            // Accumulate CompilationUnitSyntax.
            _classUnitSyntaxes.Add($"{className}.cs", compilationSyntax);

            // Refresh types.
            RefreshTypes(compilationSyntax);

            _logger.LogInformation($"[CodeGenerator] Class {className} generated.");

            return true;
        }

        /// <summary>
        /// Create files.
        /// </summary>
        public void CreateFiles(string dirPath)
        {
            // Check foler existed.
            var enumFolder = Path.Combine(dirPath, FilePath.FOLDER_ENUM);
            Directory.CreateDirectory(enumFolder);

            // Create enum.
            foreach (var pair in _enumUnitSyntaxes)
            {
                var fileName = pair.Key;
                var compilationSyntax = pair.Value;

                var filePath = Path.Combine(enumFolder, fileName);

                File.WriteAllText(filePath, compilationSyntax.ToFullString());
            }

            // Check foler existed.
            var classFolder = Path.Combine(dirPath, FilePath.CLASS_FOLDER);
            Directory.CreateDirectory(classFolder);

            // Create class.
            foreach (var pair in _classUnitSyntaxes)
            {
                var fileName = pair.Key;
                var compilationSyntax = pair.Value;

                var filePath = Path.Combine(classFolder, fileName);

                File.WriteAllText(filePath, compilationSyntax.ToFullString());
            }
        }

        /// <summary>
        /// Refresh types.
        /// </summary>
        private bool RefreshTypes(CompilationUnitSyntax compilationSyntax)
        {
            _compilation = _compilation.AddSyntaxTrees(compilationSyntax.SyntaxTree);

            using (var ms = new MemoryStream())
            {
                // emit compilation to memory stream.
                var result = _compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);

                    // Load assembly.
                    var assembly = Assembly.Load(ms.ToArray());

                    _generatedTypes = assembly.GetTypes().ToList();
                }
                else
                {
                    // Fail.
                    var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);

                    foreach (var fail in failures)
                    {
                        _logger.LogError($"enum code compilation failed. msg: {fail.ToString()}");
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create enum CodeCompileUnit\.
        /// </summary>
        /// <param name="enumNamespace">Namespace for Enum</param>
        /// <param name="enumName">Enum name</param>
        /// <param name="enumMembers">Enum member list</param>
        private CompilationUnitSyntax CreateEnumCompilationUnit(string enumNamespace, string enumName, IEnumerable<EnumMember> enumMembers)
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
                    .EnumMemberDeclaration(enumMember.Name)         // Name
                    .WithEqualsValue(                               // Value
                        SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(enumMember.Value)
                                )
                            )
                        )
                .WithLeadingTrivia(                                 // 주석 추가
                    SyntaxFactory.Comment($"/// <summary>"), SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment($"/// {enumMember.Comment}"), SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment($"/// </summary>"), SyntaxFactory.CarriageReturnLineFeed
                    );

                // Add member.
                declareEnum = declareEnum.AddMembers(memberSyntax);
            }

            // Add member.
            declareNamespace = declareNamespace.AddMembers(declareEnum);

            return SyntaxFactory.CompilationUnit().AddMembers(declareNamespace).NormalizeWhitespace();
        }


        /// <summary>
        /// Create class CodeCompileUnit.
        /// </summary>
        /// <param name="classNamespace">Namespace for class</param>
        /// <param name="className">Enum name</param>
        /// <param name="classMembers">Class member list</param>
        private CompilationUnitSyntax CreateClassCompilationUnit(string classNamespace, string className, IEnumerable<ClassMember> classMembers)
        {
            // Create namespace.
            var declareNamespace = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.IdentifierName(classNamespace));

            // Create class declare.
            var declareClass = SyntaxFactory
                .ClassDeclaration(className)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var namespaceList = new HashSet<string>();

            // Create member.
            foreach (var classMember in classMembers)
            {
                var type = FindType(classMember.Type);
                if (null == type)
                {
                    // Not found type.
                    return null;
                }

                // Add type into HashSet.
                if (false == namespaceList.Contains(type.Namespace))
                {
                    namespaceList.Add(type.Namespace);
                }

                // Field.
                var propSyntax = SyntaxFactory
                    .PropertyDeclaration(
                        SyntaxFactory.ParseTypeName(classMember.Type),                          // Type
                        classMember.Name                                                        // Name
                    )
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))                // Public
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),// get
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) // set
                    )
                    .WithLeadingTrivia(                                                         // 주석 추가
                        SyntaxFactory.Comment($"/// <summary>"), SyntaxFactory.CarriageReturnLineFeed,
                        SyntaxFactory.Comment($"/// {classMember.Comment}"), SyntaxFactory.CarriageReturnLineFeed,
                        SyntaxFactory.Comment($"/// </summary>"), SyntaxFactory.CarriageReturnLineFeed
                    );

                // Add member.
                declareClass = declareClass.AddMembers(propSyntax);
            }


            // Add using.
            foreach (var name in namespaceList)
            {
                // Add namespace for using.
                declareNamespace = declareNamespace
                    .AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(name)
                        ));
            }

            // Add member.
            declareNamespace = declareNamespace.AddMembers(declareClass);

            return SyntaxFactory.CompilationUnit().AddMembers(declareNamespace).NormalizeWhitespace();
        }

        /// <summary>
        /// Get type from name
        /// </summary>
        private Type FindType(string typeName)
        {
            // 1. Find from type table.
            if (true == _primitiveTypeDict.ContainsKey(typeName))
            {
                return _primitiveTypeDict[typeName];
            }

            // 2. Find from generator.
            var types = GetNewTypes();
            if (null != types)
            {
                return types.FirstOrDefault(d => d.Name.Equals(typeName));
            }

            return null;
        }
    }
}
