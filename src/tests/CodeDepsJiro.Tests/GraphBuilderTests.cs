using CodeDepsJiro.Models;
using GraphBuilderType = CodeDepsJiro.GraphBuilder.GraphBuilder;

namespace CodeDepsJiro.Tests;

public sealed class GraphBuilderTests
{
    [Fact]
    public void Build_DeduplicatesNodesById()
    {
        var from = new Node
        {
            Id = "A",
            Name = "A",
            Namespace = "Sample",
            Kind = NodeKind.Class,
        };

        var to = new Node
        {
            Id = "B",
            Name = "B",
            Namespace = "Sample",
            Kind = NodeKind.Class,
        };

        var edges = new List<DependencyEdge>
        {
            new()
            {
                From = from,
                To = to,
                RelationType = RelationType.Field,
            },
            new()
            {
                From = from,
                To = to,
                RelationType = RelationType.Property,
            },
        };

        var builder = new GraphBuilderType();
        var graph = builder.Build(edges);

        Assert.Equal(2, graph.Nodes.Count);
        Assert.Equal(2, graph.Edges.Count);
    }
}
