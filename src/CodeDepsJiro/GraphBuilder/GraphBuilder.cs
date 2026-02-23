using System;
using System.Collections.Generic;
using System.Linq;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.GraphBuilder;

public sealed class GraphBuilder : IGraphBuilder
{
    /// <summary>
    /// 依存エッジからノードとグラフを構築する。
    /// </summary>
    /// <param name="edges">依存エッジ一覧。</param>
    /// <returns>構築されたグラフ。</returns>
    public Graph Build(IReadOnlyList<DependencyEdge> edges)
    {
        if (edges == null)
        {
            throw new ArgumentNullException(nameof(edges));
        }

        var nodes = new Dictionary<string, Node>(StringComparer.Ordinal);
        foreach (var edge in edges)
        {
            if (!nodes.ContainsKey(edge.From.Id))
            {
                nodes[edge.From.Id] = edge.From;
            }

            if (!nodes.ContainsKey(edge.To.Id))
            {
                nodes[edge.To.Id] = edge.To;
            }
        }

        return new Graph
        {
            Nodes = nodes.Values.ToList(),
            Edges = edges.ToList()
        };
    }
}
