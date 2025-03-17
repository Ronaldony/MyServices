
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;

namespace DataDesignerServer.Core.Generator
{
    using DataDesignerServer.Core.Common;
    using DataDesignerServer.Core.Data;
    using DataDesignerServer.Core.Types;

    /// <summary>
    /// CodeGenerator.
    /// </summary>
    public sealed class CodeGenerator
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
        /// Class CodeUnitSyntax list.
        /// key: Class name.
        /// value: CodeUnitSyntax.
        /// </summary>
        private readonly Dictionary<string, CodeUnitSyntax> _classUnitSyntaxDict;

        /// <summary>
        /// Enum CodeUnitSyntax list.
        /// key: Enum name.
        /// value: CodeUnitSyntax.
        /// </summary>
        private readonly Dictionary<string, CodeUnitSyntax> _enumUnitSyntaxDict;

        /// <summary>
        /// Type table dictionary.
        /// key: type name.
        /// value: Type.
        /// </summary>
        private Dictionary<string, Type> _primitiveTypeDict;

        public CodeGenerator(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<CodeGenerator>>();

            _enumUnitSyntaxDict = new Dictionary<string, CodeUnitSyntax>();
            _classUnitSyntaxDict = new Dictionary<string, CodeUnitSyntax>();

            _primitiveTypeDict = new Dictionary<string, Type>();
            _typeList = new List<Type>();

            _generatedTypes = new List<Type>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
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

            // clear UnitSyntax.
            _classUnitSyntaxDict.Clear();
            _enumUnitSyntaxDict.Clear();

            // Initialize.
            _logger.LogInformation("CodeGenerator initialized.");
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
        public void AddEnum(Type_GenerateCode genCodeType, string enumNamespace, string enumName, IEnumerable<TypeSchemaRow> rows)
        {
            _logger.LogDebug($"//----------------------------------------------------");
            _logger.LogDebug($"[CodeGenerator] Generating enum {enumName}");

            // Get compile unit.
            var compilationSyntax = CreateEnumUnitSyntax(enumNamespace, enumName, rows);

            // Update CodeUnitSyntax.
            if (_enumUnitSyntaxDict.ContainsKey(enumName))
            {
                _enumUnitSyntaxDict[enumName].UnitSyntax = compilationSyntax;
            }
            else
            {
                _enumUnitSyntaxDict.Add(enumName, new CodeUnitSyntax
                {
                    FileName = $"{enumName}.cs",
                    UnitSyntax = compilationSyntax,
                    GenCodeType = genCodeType,
                });
            }

            // Refresh types.
            FlushTypes();

            _logger.LogDebug($"[CodeGenerator] Enum {enumName} generated.");
        }

        /// <summary>
        /// Parse class code data, then generate class code.
        /// </summary>
        public bool AddClass(Type_GenerateCode genCodeType, string classNamespace, string className, IEnumerable<ClassSchemaColumn> columns)
        {
            _logger.LogDebug($"//----------------------------------------------------");
            _logger.LogDebug($"[CodeGenerator] Generating class {className}");

            // Get compile unit.
            var compilationSyntax = CreateClassUnitSyntax(classNamespace, className, columns);

            if (null == compilationSyntax)
            {
                _logger.LogError($"[CodeGenerator] Failed to generate {className}.");
                return false;
            }

            // Update CodeUnitSyntax.
            if (_classUnitSyntaxDict.ContainsKey(className))
            {
                _classUnitSyntaxDict[className].UnitSyntax = compilationSyntax;
            }
            else
            {
                _classUnitSyntaxDict.Add(className, new CodeUnitSyntax
                {
                    FileName = $"{className}.cs",
                    UnitSyntax = compilationSyntax,
                    GenCodeType = genCodeType,
                });
            }

            // Refresh types.
            FlushTypes();

            _logger.LogInformation($"[CodeGenerator] Class {className} generated.");

            return true;
        }

        /// <summary>
        /// Create files.
        /// </summary>
        public void CreateFiles(string dirPath)
        {
            var folderDict = CreateFolders(dirPath);

            // Create enum.
            foreach (var pair in _enumUnitSyntaxDict)
            {
                var codeUnitSyntax = pair.Value;

                var folder = folderDict[codeUnitSyntax.GenCodeType];
                var filePath = Path.Combine(folder, codeUnitSyntax.FileName);

                File.WriteAllText(filePath, codeUnitSyntax.UnitSyntax.ToFullString());
            }

            // Check folder.
            var classFolder = Path.Combine(dirPath, FilePath.TABLE_FOLDER);
            Directory.CreateDirectory(classFolder);

            // Create class.
            foreach (var pair in _classUnitSyntaxDict)
            {
                var codeUnitSyntax = pair.Value;

                var folder = folderDict[codeUnitSyntax.GenCodeType];
                var filePath = Path.Combine(folder, codeUnitSyntax.FileName);

                File.WriteAllText(filePath, codeUnitSyntax.UnitSyntax.ToFullString());
            }
        }

        /// <summary>
        /// Create folders.
        /// </summary>
        private Dictionary<Type_GenerateCode, string> CreateFolders(string dirPath)
        {
            var folderDict = new Dictionary<Type_GenerateCode, string>();

            // Check folder.
            foreach (var type in Enum.GetValues<Type_GenerateCode>())
            {
                var folder = string.Empty;

                switch (type)
                {
                    case Type_GenerateCode.Type:
                        {
                            folder = Path.Combine(dirPath, FilePath.TYPE_FOLDER);
                        }
                        break;
                    case Type_GenerateCode.Configuration:
                        {
                            folder = Path.Combine(dirPath, FilePath.CONFIG_FOLDER);
                        }
                        break;
                    case Type_GenerateCode.Table:
                        {
                            folder = Path.Combine(dirPath, FilePath.TABLE_FOLDER);
                        }
                        break;
                }

                // Create directory, add folder information.
                if (false == string.IsNullOrEmpty(folder))
                {
                    Directory.CreateDirectory(folder);
                    folderDict.Add(type, folder);
                }
            }

            return folderDict;
        }

        /// <summary>
        /// Flush types.
        /// </summary>
        private bool FlushTypes()
        {
            // Create compilation.
            var dllFile = typeof(CodeGenerator).Name + ".dll";

            // Csharp compilation.
            var compilation = CSharpCompilation.Create(
                assemblyName: dllFile,
                //references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Add class tree.
            foreach (var pair in _classUnitSyntaxDict)
            {
                var codeUnitSyntax = pair.Value;

                compilation = compilation.AddSyntaxTrees(codeUnitSyntax.UnitSyntax.SyntaxTree);
            }

            // Add enum tree.
            foreach (var pair in _enumUnitSyntaxDict)
            {
                var codeUnitSyntax = pair.Value;

                compilation = compilation.AddSyntaxTrees(codeUnitSyntax.UnitSyntax.SyntaxTree);
            }

            using (var ms = new MemoryStream())
            {
                // emit compilation to memory stream.
                var result = compilation.Emit(ms);

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
        /// Create enum CodeCompileUnit.
        /// </summary>
        private CompilationUnitSyntax CreateEnumUnitSyntax(string enumNamespace, string enumName, IEnumerable<TypeSchemaRow> rows)
        {
            // Create namespace.
            var declareNamespace = SyntaxFactory
                .NamespaceDeclaration(SyntaxFactory.IdentifierName(enumNamespace));

            // Create enum declare.
            var declareEnum = SyntaxFactory
                .EnumDeclaration(enumName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            // Create enum member.
            foreach (var row in rows)
            {
                var memberSyntax = SyntaxFactory
                    .EnumMemberDeclaration(row.Name)                // Name
                    .WithEqualsValue(                               // Id
                        SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(row.Id)
                                )
                            )
                        )
                .WithLeadingTrivia(                                 // 주석 추가
                    SyntaxFactory.Comment($"/// <summary>"), SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment($"/// {row.Summary}"), SyntaxFactory.CarriageReturnLineFeed,
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
        private CompilationUnitSyntax CreateClassUnitSyntax(string classNamespace, string className, IEnumerable<ClassSchemaColumn> columns)
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
            foreach (var column in columns)
            {
                var type = FindType(column.Type);
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
                        SyntaxFactory.ParseTypeName(column.Type),                          // Type
                        column.Name                                                        // Name
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
                        SyntaxFactory.Comment($"/// {column.Summary}"), SyntaxFactory.CarriageReturnLineFeed,
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
