using CodeDepsJiro.Models;

namespace CodeDepsJiro.GraphBuilder;

public interface IGraphBuilder
{
    Graph Build(IReadOnlyList<DependencyEdge> edges);
}
