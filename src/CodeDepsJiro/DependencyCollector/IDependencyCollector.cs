using System.Collections.Generic;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.DependencyCollector;

public interface IDependencyCollector
{
    IReadOnlyList<DependencyEdge> Collect(SemanticAnalyzer.SemanticAnalysisResult semanticResult);
}
