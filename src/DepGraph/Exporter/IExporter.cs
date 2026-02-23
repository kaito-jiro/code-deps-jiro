using System.Collections.Generic;
using DepGraph.Models;

namespace DepGraph.Exporter;

public interface IExporter
{
    string Export(Graph graph, IReadOnlyList<RuleViolation> violations);
}
