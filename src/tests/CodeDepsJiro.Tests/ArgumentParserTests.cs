using CodeDepsJiro.Cli;

namespace CodeDepsJiro.Tests;

public sealed class ArgumentParserTests
{
    /// <summary>
    /// 入力パスのみ指定した場合にオプションを正しく生成できることを確認する。
    /// </summary>
    [Fact]
    public void Parse_WithInputOnly_ReturnsOptions()
    {
        var options = ArgumentParser.Parse(["./MyProject"]);

        Assert.Equal("./MyProject", options.InputPath);
        Assert.Null(options.OutputPath);
        Assert.Null(options.FilterPattern);
        Assert.Null(options.RulesFile);
        Assert.Null(options.ExcludePattern);
    }

    /// <summary>
    /// サポート対象のオプションを指定した場合に値が反映されることを確認する。
    /// </summary>
    [Fact]
    public void Parse_WithSupportedOptions_ReturnsOptions()
    {
        var options = ArgumentParser.Parse(
        [
            "./MyProject",
            "--output", "out/dependencies.json",
            "--filter", "ns:*UI*",
            "--rules", "rules.json",
            "--exclude", "*Tests*",
        ]);

        Assert.Equal("./MyProject", options.InputPath);
        Assert.Equal("out/dependencies.json", options.OutputPath);
        Assert.Equal("ns:*UI*", options.FilterPattern);
        Assert.Equal("rules.json", options.RulesFile);
        Assert.Equal("*Tests*", options.ExcludePattern);
    }

    /// <summary>
    /// 削除済みの --format オプションを指定した場合に未知オプションとして扱われることを確認する。
    /// </summary>
    [Fact]
    public void Parse_WithFormatOption_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(["./MyProject", "--format", "json"]));

        Assert.Equal("Unknown option: --format", exception.Message);
    }
}
