using System.Collections.Generic;
using System.Linq;
using DepGraph.Models;

namespace DepGraph.Exporter;

public sealed class DotExporter : IExporter
{
    public string Export(Graph graph, IReadOnlyList<RuleViolation> violations)
    {
        var lines = graph.Edges.Select(edge => $"    {edge.From.Name} -> {edge.To.Name};");
        return "digraph G {\n" + string.Join("\n", lines) + "\n}";
    }
}
