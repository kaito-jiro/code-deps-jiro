using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DepGraph.SemanticAnalyzer;

public sealed class SemanticAnalysisResult
{
    public SyntaxAnalyzer.SyntaxAnalysisResult SyntaxResult { get; init; } = new();
    public IReadOnlyList<SyntaxTree> SyntaxTrees { get; init; } = new List<SyntaxTree>();
    public Compilation Compilation { get; init; } = null!;
}
