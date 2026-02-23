using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DepGraph.SemanticAnalyzer;

public sealed class SemanticAnalyzer : ISemanticAnalyzer
{
    /// <summary>
    /// 構文解析結果から Roslyn の Compilation を構築する。
    /// </summary>
    /// <param name="syntaxResult">構文解析結果。</param>
    /// <returns>セマンティック解析結果。</returns>
    public SemanticAnalysisResult Analyze(SyntaxAnalyzer.SyntaxAnalysisResult syntaxResult)
    {
        if (syntaxResult == null)
        {
            throw new ArgumentNullException(nameof(syntaxResult));
        }

        var trees = syntaxResult.SourceFiles
            .Select(file => CSharpSyntaxTree.ParseText(file.Content, path: file.Path))
            .ToList();

        var compilation = CSharpCompilation.Create(
            assemblyName: "DepGraph.Analysis",
            syntaxTrees: trees,
            references: GetDefaultReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return new SemanticAnalysisResult
        {
            SyntaxResult = syntaxResult,
            SyntaxTrees = trees,
            Compilation = compilation,
        };
    }

    /// <summary>
    /// 基本的なアセンブリ参照を解決する。
    /// </summary>
    /// <returns>メタデータ参照の一覧。</returns>
    private static IEnumerable<MetadataReference> GetDefaultReferences()
    {
        var locations = new HashSet<string>
        {
            typeof(object).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(System.Runtime.GCSettings).Assembly.Location,
        };

        return locations.Select(path => MetadataReference.CreateFromFile(path));
    }
}
