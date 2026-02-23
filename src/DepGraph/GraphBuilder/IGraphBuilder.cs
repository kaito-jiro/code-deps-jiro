using DepGraph.Models;

namespace DepGraph.GraphBuilder;

public interface IGraphBuilder
{
    Graph Build(IReadOnlyList<DependencyEdge> edges);
}
