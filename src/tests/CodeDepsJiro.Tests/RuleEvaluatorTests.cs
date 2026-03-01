using CodeDepsJiro.Models;
using RuleEvaluatorType = CodeDepsJiro.RuleEvaluator.RuleEvaluator;

namespace CodeDepsJiro.Tests;

public sealed class RuleEvaluatorTests
{
    /// <summary>
    /// ルール違反がある場合に違反が検出されることを確認する。
    /// </summary>
    [Fact]
    public void Evaluate_WithMatchingViolation_ReturnsViolation()
    {
        var graph = CreateGraph(
            CreateEdge("MyApp.Application", "MyApp.Infrastructure"));
        var ruleSet = CreateRuleSet(
            ["Application", "Infrastructure"],
            [
                new ViolationRule { From = "Application", To = "Infrastructure" }
            ]);

        var evaluator = new RuleEvaluatorType();
        var violations = evaluator.Evaluate(graph, ruleSet);

        Assert.Single(violations);
        Assert.Equal("Application", violations[0].FromLayer);
        Assert.Equal("Infrastructure", violations[0].ToLayer);
    }

    /// <summary>
    /// 違反ルールに該当しない場合は違反が返らないことを確認する。
    /// </summary>
    [Fact]
    public void Evaluate_WithNoMatchingViolation_ReturnsEmpty()
    {
        var graph = CreateGraph(
            CreateEdge("MyApp.Domain", "MyApp.Application"));
        var ruleSet = CreateRuleSet(
            ["Domain", "Application"],
            [
                new ViolationRule { From = "Application", To = "Infrastructure" }
            ]);

        var evaluator = new RuleEvaluatorType();
        var violations = evaluator.Evaluate(graph, ruleSet);

        Assert.Empty(violations);
    }

    /// <summary>
    /// レイヤーに一致しない名前空間は評価対象から除外されることを確認する。
    /// </summary>
    [Fact]
    public void Evaluate_WithUnmappedNamespace_ReturnsEmpty()
    {
        var graph = CreateGraph(
            CreateEdge("ThirdParty.Lib", "MyApp.Infrastructure"));
        var ruleSet = CreateRuleSet(
            ["Application", "Infrastructure"],
            [
                new ViolationRule { From = "Application", To = "Infrastructure" }
            ]);

        var evaluator = new RuleEvaluatorType();
        var violations = evaluator.Evaluate(graph, ruleSet);

        Assert.Empty(violations);
    }

    /// <summary>
    /// ルールが未設定の場合に空の結果となることを確認する。
    /// </summary>
    [Fact]
    public void Evaluate_WithEmptyRuleSet_ReturnsEmpty()
    {
        var graph = CreateGraph(
            CreateEdge("MyApp.Application", "MyApp.Infrastructure"));
        var ruleSet = new RuleSet();

        var evaluator = new RuleEvaluatorType();
        var violations = evaluator.Evaluate(graph, ruleSet);

        Assert.Empty(violations);
    }

    /// <summary>
    /// 依存グラフを生成する。
    /// </summary>
    private static Graph CreateGraph(params DependencyEdge[] edges)
    {
        return new Graph
        {
            Nodes = new List<Node>(),
            Edges = edges,
        };
    }

    /// <summary>
    /// 依存エッジを生成する。
    /// </summary>
    private static DependencyEdge CreateEdge(string fromNamespace, string toNamespace)
    {
        return new DependencyEdge
        {
            From = CreateNode(fromNamespace),
            To = CreateNode(toNamespace),
            RelationType = RelationType.Field,
        };
    }

    /// <summary>
    /// ノードを生成する。
    /// </summary>
    private static Node CreateNode(string namespaceName)
    {
        var name = namespaceName.Split('.').Last();
        return new Node
        {
            Id = $"{namespaceName}.Type",
            Name = name,
            Namespace = namespaceName,
            Kind = NodeKind.Class,
        };
    }

    /// <summary>
    /// レイヤーと違反ルールからルールセットを生成する。
    /// </summary>
    private static RuleSet CreateRuleSet(
        IEnumerable<string> layerNames,
        IEnumerable<ViolationRule> violations)
    {
        var layers = new List<LayerRule>();
        foreach (var layer in layerNames)
        {
            layers.Add(new LayerRule
            {
                Name = layer,
                Patterns = [$"MyApp.{layer}*"],
            });
        }

        return new RuleSet
        {
            Layers = layers,
            Violations = violations.ToList(),
        };
    }
}
