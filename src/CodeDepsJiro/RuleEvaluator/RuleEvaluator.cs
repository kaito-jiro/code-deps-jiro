using System;
using System.Collections.Generic;
using CodeDepsJiro.Models;

namespace CodeDepsJiro.RuleEvaluator;

public sealed class RuleEvaluator : IRuleEvaluator
{
    /// <summary>
    /// 依存グラフに対してレイヤールール違反を評価する。
    /// </summary>
    /// <param name="graph">依存グラフ。</param>
    /// <param name="ruleSet">ルール定義。</param>
    /// <returns>検出された違反一覧。</returns>
    public IReadOnlyList<RuleViolation> Evaluate(Graph graph, RuleSet ruleSet)
    {
        if (graph == null)
        {
            throw new ArgumentNullException(nameof(graph));
        }

        if (ruleSet == null)
        {
            throw new ArgumentNullException(nameof(ruleSet));
        }

        if (ruleSet.Layers.Count == 0 || ruleSet.Violations.Count == 0)
        {
            return Array.Empty<RuleViolation>();
        }

        var layerMap = BuildLayerMap(ruleSet.Layers);
        var violations = new List<RuleViolation>();

        foreach (var edge in graph.Edges)
        {
            var fromLayer = ResolveLayer(layerMap, edge.From.Namespace);
            var toLayer = ResolveLayer(layerMap, edge.To.Namespace);

            if (fromLayer == null || toLayer == null)
            {
                continue;
            }

            foreach (var rule in ruleSet.Violations)
            {
                if (string.Equals(rule.From, fromLayer, StringComparison.Ordinal) &&
                    string.Equals(rule.To, toLayer, StringComparison.Ordinal))
                {
                    violations.Add(new RuleViolation
                    {
                        FromLayer = fromLayer,
                        ToLayer = toLayer,
                        Edge = edge,
                    });
                }
            }
        }

        return violations;
    }

    /// <summary>
    /// レイヤー名とパターンをマップ化する。
    /// </summary>
    /// <param name="layers">レイヤールール。</param>
    /// <returns>レイヤーマップ。</returns>
    private static Dictionary<string, IReadOnlyList<string>> BuildLayerMap(IReadOnlyList<LayerRule> layers)
    {
        var map = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        foreach (var layer in layers)
        {
            map[layer.Name] = layer.Patterns;
        }

        return map;
    }

    /// <summary>
    /// 名前空間に対応するレイヤー名を解決する。
    /// </summary>
    /// <param name="layerMap">レイヤーマップ。</param>
    /// <param name="namespaceName">名前空間。</param>
    /// <returns>一致したレイヤー名。未一致の場合は null。</returns>
    private static string? ResolveLayer(
        IReadOnlyDictionary<string, IReadOnlyList<string>> layerMap,
        string namespaceName)
    {
        foreach (var (layerName, patterns) in layerMap)
        {
            foreach (var pattern in patterns)
            {
                if (WildcardMatch(pattern, namespaceName))
                {
                    return layerName;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// ワイルドカード（* / ?）を含む簡易マッチを行う。
    /// </summary>
    /// <param name="pattern">パターン。</param>
    /// <param name="text">判定対象。</param>
    /// <returns>一致する場合 true。</returns>
    private static bool WildcardMatch(string pattern, string text)
    {
        var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$";
        return System.Text.RegularExpressions.Regex.IsMatch(text, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}
