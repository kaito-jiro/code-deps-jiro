using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DepGraph.SyntaxAnalyzer;

public sealed class SyntaxAnalyzer : ISyntaxAnalyzer
{
    /// <summary>
    /// C# ソースファイルを解析し、型宣言情報を抽出する。
    /// </summary>
    /// <param name="sourceFiles">解析対象の C# ソースファイルパス一覧。</param>
    /// <returns>構文解析結果（ソースと型宣言）。</returns>
    public SyntaxAnalysisResult Analyze(IReadOnlyList<string> sourceFiles)
    {
        if (sourceFiles == null)
        {
            throw new ArgumentNullException(nameof(sourceFiles));
        }

        var loadedFiles = sourceFiles.Select(path => new SourceFile
            {
                Path = path,
                Content = File.ReadAllText(path)
            })
            .ToList();

        var typeDeclarations = new List<TypeDeclarationInfo>();
        foreach (var file in loadedFiles)
        {
            var tree = CSharpSyntaxTree.ParseText(file.Content);
            var root = tree.GetRoot();
            var declarations = root.DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>();

            foreach (var declaration in declarations)
            {
                if (declaration is not ClassDeclarationSyntax && declaration is not InterfaceDeclarationSyntax)
                {
                    continue;
                }

                var kind = GetTypeKind(declaration);
                var namespaceName = GetNamespaceName(declaration);
                var name = declaration.Identifier.Text;

                typeDeclarations.Add(new TypeDeclarationInfo
                {
                    Name = name,
                    Namespace = namespaceName,
                    Kind = kind,
                });
            }
        }

        return new SyntaxAnalysisResult
        {
            SourceFiles = loadedFiles,
            TypeDeclarations = typeDeclarations
        };
    }

    /// <summary>
    /// 型宣言の種別を判定する。
    /// </summary>
    /// <param name="declaration">型宣言ノード。</param>
    /// <returns>型の種別。</returns>
    private static TypeKind GetTypeKind(BaseTypeDeclarationSyntax declaration)
    {
        if (declaration is InterfaceDeclarationSyntax)
        {
            return TypeKind.Interface;
        }

        if (declaration.Modifiers.Any(SyntaxKind.AbstractKeyword))
        {
            return TypeKind.AbstractClass;
        }

        return TypeKind.Class;
    }

    /// <summary>
    /// 型宣言が属する名前空間を解決する。
    /// </summary>
    /// <param name="declaration">型宣言ノード。</param>
    /// <returns>名前空間名。ルートの場合は空文字。</returns>
    private static string GetNamespaceName(SyntaxNode declaration)
    {
        for (var current = declaration.Parent; current != null; current = current.Parent)
        {
            if (current is FileScopedNamespaceDeclarationSyntax fileScoped)
            {
                return fileScoped.Name.ToString();
            }

            if (current is NamespaceDeclarationSyntax ns)
            {
                return ns.Name.ToString();
            }
        }

        return string.Empty;
    }
}
