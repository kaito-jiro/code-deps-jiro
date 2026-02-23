namespace DepGraph.Cli;

public sealed class Options
{
    public required string InputPath { get; set; }
    public bool OutputDot { get; set; }
    public string? FilterPattern { get; set; }
    public string? RulesFile { get; set; }
    public string? ExcludePattern { get; set; }
}
