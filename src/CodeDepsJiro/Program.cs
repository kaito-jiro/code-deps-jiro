using System;
using System.IO;
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

    var sourceFiles = projectLoader.LoadSourceFiles(options.InputPath, options.ExcludePattern);
    var syntaxResult = syntaxAnalyzer.Analyze(sourceFiles);
    var semanticResult = semanticAnalyzer.Analyze(syntaxResult);
    var dependencies = dependencyCollector.Collect(semanticResult);
    var graph = graphBuilder.Build(dependencies);

    // TODO: Load rules from options.RulesFile when implemented.
    var violations = ruleEvaluator.Evaluate(graph, new RuleSet());

    IExporter exporter = options.OutputFormat switch
    {
        OutputFormat.Json => new JsonExporter(),
        OutputFormat.Csv => new CsvExporter(),
        _ => new PlainTextExporter(),
    };
    var output = exporter.Export(graph, violations);

    if (string.IsNullOrWhiteSpace(options.OutputPath))
    {
        Console.WriteLine(output);
    }
    else
    {
        var directory = Path.GetDirectoryName(options.OutputPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(options.OutputPath, output);
    }
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    Console.Error.WriteLine("Usage: CodeDepsJiro <path> [--format <plain|json|csv>] [--output <file>] [--filter <pattern>] [--rules <file>] [--exclude <pattern>]");
    Environment.Exit(1);
}
