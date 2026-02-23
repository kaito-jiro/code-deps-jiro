using System;
using DepGraph.Cli;
using DepGraph.DependencyCollector;
using DepGraph.Exporter;
using DepGraph.GraphBuilder;
using DepGraph.Models;
using DepGraph.ProjectLoader;
using DepGraph.RuleEvaluator;
using DepGraph.SemanticAnalyzer;
using DepGraph.SyntaxAnalyzer;

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

    IExporter exporter = options.OutputDot ? new DotExporter() : new PlainTextExporter();
    var output = exporter.Export(graph, violations);
    Console.WriteLine(output);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    Console.Error.WriteLine("Usage: depgraph <path> [--dot] [--filter <pattern>] [--rules <file>] [--exclude <pattern>]");
    Environment.Exit(1);
}
