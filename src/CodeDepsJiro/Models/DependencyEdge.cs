namespace CodeDepsJiro.Models;

public sealed class DependencyEdge
{
    public required Node From { get; init; }
    public required Node To { get; init; }
    public required RelationType RelationType { get; init; }
}

public enum RelationType
{
    Inherits,
    Implements,
    Field,
    Property,
    Parameter,
    Return,
    New,
}
