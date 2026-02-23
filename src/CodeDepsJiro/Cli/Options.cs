namespace CodeDepsJiro.Cli;

public sealed class Options
{
    public required string InputPath { get; set; }
    public OutputFormat OutputFormat { get; set; } = OutputFormat.Plain;
    public string? OutputPath { get; set; }
    public string? FilterPattern { get; set; }
    public string? RulesFile { get; set; }
    public string? ExcludePattern { get; set; }
}

public enum OutputFormat
{
    Plain,
    Json,
    Csv,
}
