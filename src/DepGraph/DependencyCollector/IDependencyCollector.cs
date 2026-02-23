using System.Collections.Generic;
using DepGraph.Models;

namespace DepGraph.DependencyCollector;

public interface IDependencyCollector
{
    IReadOnlyList<DependencyEdge> Collect(SemanticAnalyzer.SemanticAnalysisResult semanticResult);
}
