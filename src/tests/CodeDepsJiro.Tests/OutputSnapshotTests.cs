using CodeDepsJiro.Exporter;
using CodeDepsJiro.Models;
using DependencyCollectorType = CodeDepsJiro.DependencyCollector.DependencyCollector;
using GraphBuilderType = CodeDepsJiro.GraphBuilder.GraphBuilder;
using SemanticAnalyzerType = CodeDepsJiro.SemanticAnalyzer.SemanticAnalyzer;
using SyntaxAnalyzerType = CodeDepsJiro.SyntaxAnalyzer.SyntaxAnalyzer;

namespace CodeDepsJiro.Tests;

public sealed class OutputSnapshotTests
{
    [Fact]
    public void JsonOutput_MatchesSnapshot()
    {
        var filePath = WriteTestFile(GetSampleCode());

        try
        {
            var output = GenerateOutput(filePath, new JsonExporter());
            var normalized = NormalizeNewlines(output);
            var expected = NormalizeNewlines(ReadSnapshot("dependencies.json"));

            Assert.Equal(expected, normalized);
        }
        finally
        {
            DeleteTestFile(filePath);
        }
    }

    [Fact]
    public void CsvOutput_MatchesSnapshot()
    {
        var filePath = WriteTestFile(GetSampleCode());

        try
        {
            var output = GenerateOutput(filePath, new CsvExporter());
            var normalized = NormalizeNewlines(output);
            var expected = NormalizeNewlines(ReadSnapshot("dependencies.csv"));

            Assert.Equal(expected, normalized);
        }
        finally
        {
            DeleteTestFile(filePath);
        }
    }

    private static string GenerateOutput(string filePath, IExporter exporter)
    {
        var syntaxAnalyzer = new SyntaxAnalyzerType();
        var semanticAnalyzer = new SemanticAnalyzerType();
        var collector = new DependencyCollectorType();
        var graphBuilder = new GraphBuilderType();

        var syntaxResult = syntaxAnalyzer.Analyze([filePath]);
        var semanticResult = semanticAnalyzer.Analyze(syntaxResult);
        var edges = collector.Collect(semanticResult);
        var graph = graphBuilder.Build(edges);

        return exporter.Export(graph, new List<RuleViolation>());
    }

    private static string GetSampleCode()
    {
        return """
namespace Sample;

public class Dependency {}

public class Target
{
    private Dependency _field;
    public Dependency Prop { get; }

    public Dependency Method(Dependency arg)
    {
        var local = new Dependency();
        return arg;
    }
}
""";
    }

    private static string ReadSnapshot(string fileName)
    {
        var baseDirectory = AppContext.BaseDirectory;
        var snapshotsDirectory = Path.Combine(baseDirectory, "Snapshots");
        var snapshotPath = Path.Combine(snapshotsDirectory, fileName);

        if (!File.Exists(snapshotPath))
        {
            throw new FileNotFoundException($"Snapshot not found: {snapshotPath}");
        }

        return File.ReadAllText(snapshotPath);
    }

    private static string WriteTestFile(string code)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "TestData");
        Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(filePath, code);
        return filePath;
    }

    private static void DeleteTestFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private static string NormalizeNewlines(string value)
    {
        return value.Replace("\r\n", "\n").TrimEnd('\n');
    }
}
