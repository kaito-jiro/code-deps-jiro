using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.Exporter;

public sealed class JsonExporter : IExporter
{
    /// <summary>
    /// 解析結果を JSON 形式で出力します。
    /// </summary>
    /// <param name="graph">依存グラフ。</param>
    /// <param name="violations">ルール違反一覧。</param>
    /// <returns>JSON 文字列。</returns>
    public string Export(Graph graph, IReadOnlyList<RuleViolation> violations)
    {
        var payload = new
        {
            nodes = graph.Nodes.Select(node => new
            {
                name = node.Name,
                kind = node.Kind.ToString(),
            }),
            edges = graph.Edges.Select(edge => new
            {
                from = edge.From.Name,
                to = edge.To.Name,
                relationType = edge.RelationType.ToString(),
            }),
            violations = violations.Select(violation => new
            {
                fromLayer = violation.FromLayer,
                toLayer = violation.ToLayer,
                from = violation.Edge.From.Name,
                to = violation.Edge.To.Name,
                relationType = violation.Edge.RelationType.ToString(),
            }),
        };

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
    }
}
