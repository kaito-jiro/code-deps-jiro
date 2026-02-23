using System.Collections.Generic;

namespace CodeDepsJiro.SyntaxAnalyzer;

public sealed class SyntaxAnalysisResult
{
    public IReadOnlyList<SourceFile> SourceFiles { get; init; } = new List<SourceFile>();
    public IReadOnlyList<TypeDeclarationInfo> TypeDeclarations { get; init; } = new List<TypeDeclarationInfo>();
}

public sealed class SourceFile
{
    public required string Path { get; init; }
    public required string Content { get; init; }
}

public sealed class TypeDeclarationInfo
{
    public required string Name { get; init; }
    public required string Namespace { get; init; }
    public required TypeKind Kind { get; init; }
}

public enum TypeKind
{
    Class,
    Interface,
    AbstractClass,
}
