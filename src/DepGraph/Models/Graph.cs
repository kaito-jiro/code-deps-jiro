using System.Collections.Generic;

namespace DepGraph.Models;

public sealed class Graph
{
    public IReadOnlyList<Node> Nodes { get; init; } = new List<Node>();
    public IReadOnlyList<DependencyEdge> Edges { get; init; } = new List<DependencyEdge>();
}
