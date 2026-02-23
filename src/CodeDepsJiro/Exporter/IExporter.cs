using System.Collections.Generic;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.Exporter;

public interface IExporter
{
    string Export(Graph graph, IReadOnlyList<RuleViolation> violations);
}
