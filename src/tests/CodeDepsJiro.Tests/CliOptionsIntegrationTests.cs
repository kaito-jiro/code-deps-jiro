using System.Diagnostics;
using System.Text.Json;
using CodeDepsJiro.Cli;

namespace CodeDepsJiro.Tests;

public sealed class CliOptionsIntegrationTests
{
    /// <summary>
    /// 入力パスのみ指定した場合に正常終了し、標準出力へ JSON が出力されることを確認する。
    /// </summary>
    [Fact]
    public async Task Run_WithInputOnly_WritesJsonToStdOutAsync()
    {
        var root = CreateTempDirectory();
        try
        {
            var projectPath = CreateSampleProject(root);

            var result = await RunCliAsync([projectPath]);

            Assert.Equal(0, result.ExitCode);
            Assert.False(string.IsNullOrWhiteSpace(result.StdOut));
            Assert.True(IsValidJson(result.StdOut));
            Assert.True(string.IsNullOrWhiteSpace(result.StdErr));
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    /// <summary>
    /// --output 指定時に JSON ファイルが生成されることを確認する。
    /// </summary>
    [Fact]
    public async Task Run_WithOutputOption_CreatesOutputFileAsync()
    {
        var root = CreateTempDirectory();
        try
        {
            var projectPath = CreateSampleProject(root);
            var outputPath = Path.Combine(root, "out", "dependencies.json");

            var result = await RunCliAsync([projectPath, "--output", outputPath]);

            Assert.Equal(0, result.ExitCode);
            Assert.True(File.Exists(outputPath));
            var output = File.ReadAllText(outputPath);
            Assert.True(IsValidJson(output));
            Assert.True(string.IsNullOrWhiteSpace(result.StdErr));
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    /// <summary>
    /// --rules / --filter / --exclude を併用しても実行できることを確認する。
    /// </summary>
    [Fact]
    public async Task Run_WithRulesFilterExclude_CompletesSuccessfullyAsync()
    {
        var root = CreateTempDirectory();
        try
        {
            var projectPath = CreateSampleProject(root);
            var rulesPath = WriteRulesFile(Path.Combine(root, "rules.json"));

            var result = await RunCliAsync(
            [
                projectPath,
                "--rules", rulesPath,
                "--filter", "ns:*Sample*",
                "--exclude", "*Ignored.cs",
            ]);

            Assert.Equal(0, result.ExitCode);
            Assert.True(IsValidJson(result.StdOut));
            Assert.True(string.IsNullOrWhiteSpace(result.StdErr));
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    /// <summary>
    /// 未知オプション指定時に失敗し、エラーメッセージが表示されることを確認する。
    /// </summary>
    [Fact]
    public async Task Run_WithUnknownOption_ReturnsErrorAsync()
    {
        var root = CreateTempDirectory();
        try
        {
            var projectPath = CreateSampleProject(root);

            var result = await RunCliAsync([projectPath, "--unknown", "value"]);

            Assert.Equal(1, result.ExitCode);
            Assert.Contains("Unknown option: --unknown", result.StdErr, StringComparison.Ordinal);
            Assert.Contains("Usage: CodeDepsJiro", result.StdErr, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    /// <summary>
    /// 必須引数不足時に失敗し、Usage が表示されることを確認する。
    /// </summary>
    [Fact]
    public async Task Run_WithoutInputPath_ReturnsErrorAsync()
    {
        var result = await RunCliAsync([]);

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Input path is required.", result.StdErr, StringComparison.Ordinal);
        Assert.Contains("Usage: CodeDepsJiro", result.StdErr, StringComparison.Ordinal);
    }

    /// <summary>
    /// テスト用の最小構成プロジェクトを作成する。
    /// </summary>
    /// <param name="root">作成先ディレクトリ。</param>
    /// <returns>作成した .csproj のパス。</returns>
    private static string CreateSampleProject(string root)
    {
        var projectDir = Path.Combine(root, "Sample");
        Directory.CreateDirectory(projectDir);

        WriteFile(Path.Combine(projectDir, "Program.cs"), """
namespace Sample;
public sealed class App
{
    public Dependency Dep { get; } = new();
}
""");

        WriteFile(Path.Combine(projectDir, "Dependency.cs"), """
namespace Sample;
public sealed class Dependency {}
""");

        WriteFile(Path.Combine(projectDir, "Ignored.cs"), """
namespace Sample;
public sealed class Ignored {}
""");

        var projectPath = Path.Combine(projectDir, "Sample.csproj");
        WriteFile(projectPath, """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
""");

        return projectPath;
    }

    /// <summary>
    /// テスト用のルールファイルを書き込む。
    /// </summary>
    /// <param name="path">書き込み先パス。</param>
    /// <returns>作成したルールファイルのパス。</returns>
    private static string WriteRulesFile(string path)
    {
        WriteFile(path, """
{
  "layers": [
    { "name": "Sample", "patterns": ["Sample.*"] }
  ],
  "violations": []
}
""");
        return path;
    }

    /// <summary>
    /// CLI を実行して標準出力/標準エラーと終了コードを返す。
    /// </summary>
    /// <param name="args">CLI 引数。</param>
    /// <returns>実行結果。</returns>
    private static async Task<CliExecutionResult> RunCliAsync(IReadOnlyList<string> args)
    {
        var assemblyPath = typeof(Options).Assembly.Location;
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        startInfo.ArgumentList.Add(assemblyPath);
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start CodeDepsJiro process.");
        var stdOutTask = process.StandardOutput.ReadToEndAsync();
        var stdErrTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();
        var stdOut = await stdOutTask;
        var stdErr = await stdErrTask;

        return new CliExecutionResult(process.ExitCode, stdOut, stdErr);
    }

    /// <summary>
    /// 文字列が JSON として妥当か判定する。
    /// </summary>
    /// <param name="content">判定対象文字列。</param>
    /// <returns>妥当な JSON の場合 true。</returns>
    private static bool IsValidJson(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        try
        {
            using var _ = JsonDocument.Parse(content);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// テスト用ディレクトリを作成する。
    /// </summary>
    /// <returns>作成したディレクトリのパス。</returns>
    private static string CreateTempDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "CodeDepsJiroTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    /// <summary>
    /// ファイルを作成する。
    /// </summary>
    /// <param name="path">ファイルパス。</param>
    /// <param name="content">ファイル内容。</param>
    private static void WriteFile(string path, string content)
    {
        var parent = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(parent))
        {
            Directory.CreateDirectory(parent);
        }

        File.WriteAllText(path, content);
    }

    /// <summary>
    /// テスト用ディレクトリを削除する。
    /// </summary>
    /// <param name="path">削除対象ディレクトリ。</param>
    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    /// <summary>
    /// CLI 実行結果を保持する。
    /// </summary>
    /// <param name="ExitCode">終了コード。</param>
    /// <param name="StdOut">標準出力。</param>
    /// <param name="StdErr">標準エラー出力。</param>
    private sealed record CliExecutionResult(int ExitCode, string StdOut, string StdErr);
}
