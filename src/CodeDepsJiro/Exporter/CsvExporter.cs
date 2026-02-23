using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.Exporter;

public sealed class CsvExporter : IExporter
{
    /// <summary>
    /// 解析結果を CSV 形式で出力します。
    /// </summary>
    /// <param name="graph">依存グラフ。</param>
    /// <param name="violations">ルール違反一覧（CSV では未使用）。</param>
    /// <returns>CSV 文字列。</returns>
    public string Export(Graph graph, IReadOnlyList<RuleViolation> violations)
    {
        var builder = new StringBuilder();
        builder.AppendLine("From,To,RelationType");

        foreach (var edge in graph.Edges)
        {
            builder.Append(EscapeCsv(edge.From.Name));
            builder.Append(',');
            builder.Append(EscapeCsv(edge.To.Name));
            builder.Append(',');
            builder.AppendLine(EscapeCsv(edge.RelationType.ToString()));
        }

        return builder.ToString();
    }

    /// <summary>
    /// CSV で使用できるように文字列をエスケープします。
    /// </summary>
    /// <param name="value">対象文字列。</param>
    /// <returns>エスケープ済み文字列。</returns>
    private static string EscapeCsv(string value)
    {
        if (value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) < 0)
        {
            return value;
        }

        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }
}
