using CodeDepsJiro.Models;
using DependencyCollectorType = CodeDepsJiro.DependencyCollector.DependencyCollector;
using SemanticAnalyzerType = CodeDepsJiro.SemanticAnalyzer.SemanticAnalyzer;
using SyntaxAnalyzerType = CodeDepsJiro.SyntaxAnalyzer.SyntaxAnalyzer;

namespace CodeDepsJiro.Tests;

public sealed class DependencyCollectorTests
{
    [Fact]
    public void Collect_IncludesExpectedRelations()
    {
        var code = """
namespace Sample;

public interface IService {}
public class Base {}
public class Dependency {}
public class Other {}

public class Target : Base, IService
{
    private Dependency _field;
    public Dependency Prop { get; }

    public Other Method(Dependency arg)
    {
        var local = new Dependency();
        return new Other();
    }
}
""";

        var filePath = WriteTestFile(code);

        try
        {
            var syntaxAnalyzer = new SyntaxAnalyzerType();
            var semanticAnalyzer = new SemanticAnalyzerType();
            var collector = new DependencyCollectorType();

            var syntaxResult = syntaxAnalyzer.Analyze([filePath]);
            var semanticResult = semanticAnalyzer.Analyze(syntaxResult);

            var edges = collector.Collect(semanticResult);

            AssertEdge(edges, "Target", "Base", RelationType.Inherits);
            AssertEdge(edges, "Target", "IService", RelationType.Implements);
            AssertEdge(edges, "Target", "Dependency", RelationType.Field);
            AssertEdge(edges, "Target", "Dependency", RelationType.Property);
            AssertEdge(edges, "Target", "Dependency", RelationType.Parameter);
            AssertEdge(edges, "Target", "Other", RelationType.Return);
            AssertEdge(edges, "Target", "Dependency", RelationType.New);
            AssertEdge(edges, "Target", "Other", RelationType.New);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private static void AssertEdge(
        IReadOnlyList<DependencyEdge> edges,
        string from,
        string to,
        RelationType relation)
    {
        Assert.Contains(edges, edge =>
            edge.From.Name == from
            && edge.To.Name == to
            && edge.RelationType == relation);
    }

    private static string WriteTestFile(string code)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "TestData");
        Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(filePath, code);
        return filePath;
    }
}
