namespace CodeDepsJiro.Models;

public sealed class Node
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Namespace { get; init; }
    public required NodeKind Kind { get; init; }
}

public enum NodeKind
{
    Class,
    Interface,
    Abstract,
    Namespace,
}
