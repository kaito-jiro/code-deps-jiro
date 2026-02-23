using System.Collections.Generic;

namespace CodeDepsJiro.Models;

public sealed class RuleSet
{
    public IReadOnlyList<LayerRule> Layers { get; init; } = new List<LayerRule>();
    public IReadOnlyList<ViolationRule> Violations { get; init; } = new List<ViolationRule>();
}

public sealed class LayerRule
{
    public required string Name { get; init; }
    public IReadOnlyList<string> Patterns { get; init; } = new List<string>();
}

public sealed class ViolationRule
{
    public required string From { get; init; }
    public required string To { get; init; }
}

public sealed class RuleViolation
{
    public required string FromLayer { get; init; }
    public required string ToLayer { get; init; }
    public required DependencyEdge Edge { get; init; }
}
