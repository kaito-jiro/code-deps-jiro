using System.Collections.Generic;
using System.Linq;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.Exporter;

public sealed class PlainTextExporter : IExporter
{
    public string Export(Graph graph, IReadOnlyList<RuleViolation> violations)
    {
        return string.Join("\n", graph.Edges.Select(edge => $"{edge.From.Name} -> {edge.To.Name}"));
    }
}
