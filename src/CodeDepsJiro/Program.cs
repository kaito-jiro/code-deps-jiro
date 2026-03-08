using CodeDepsJiro.Cli;
using CodeDepsJiro.DependencyCollector;
using CodeDepsJiro.Exporter;
using CodeDepsJiro.GraphBuilder;
using CodeDepsJiro.Models;
using CodeDepsJiro.ProjectLoader;
using CodeDepsJiro.RuleEvaluator;
using CodeDepsJiro.SemanticAnalyzer;
using CodeDepsJiro.SyntaxAnalyzer;

try
{
    var options = ArgumentParser.Parse(args);

    var projectLoader = new ProjectLoader();
    var syntaxAnalyzer = new SyntaxAnalyzer();
    var semanticAnalyzer = new SemanticAnalyzer();
    var dependencyCollector = new DependencyCollector();
    var graphBuilder = new GraphBuilder();
    var ruleEvaluator = new RuleEvaluator();

    // 解析対象のソースファイルを収集
    var sourceFiles = projectLoader.LoadSourceFiles(options.InputPath, options.ExcludePattern);
    var syntaxResult = syntaxAnalyzer.Analyze(sourceFiles);
    var semanticResult = semanticAnalyzer.Analyze(syntaxResult);
    var dependencies = dependencyCollector.Collect(semanticResult);
    var graph = graphBuilder.Build(dependencies);

    // ルールファイル指定時は読み込み、未指定時は空のルールセットを使用
    var ruleSet = string.IsNullOrWhiteSpace(options.RulesFile)
        ? new RuleSet()
        : RuleSetLoader.LoadFromFile(options.RulesFile);
    var violations = ruleEvaluator.Evaluate(graph, ruleSet);

    // 出力は JSON のみ対応
    IExporter exporter = new JsonExporter();
    var output = exporter.Export(graph, violations);

    if (string.IsNullOrWhiteSpace(options.OutputPath))
    {
        // 標準出力へ出力
        Console.WriteLine(output);
    }
    else
    {
        var directory = Path.GetDirectoryName(options.OutputPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // ファイルへ出力
        File.WriteAllText(options.OutputPath, output);
    }
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    Console.Error.WriteLine("Usage: CodeDepsJiro <path> [--format <json>] [--output <file>] [--filter <pattern>] [--rules <file>] [--exclude <pattern>]");
    Environment.Exit(1);
}
