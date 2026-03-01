using CodeDepsJiro.RuleEvaluator;

namespace CodeDepsJiro.Tests;

public sealed class RuleSetLoaderTests
{
    /// <summary>
    /// 正常系の JSON を読み込んだ場合に RuleSet が構築されることを確認する。
    /// </summary>
    [Fact]
    public void LoadFromFile_WithValidJson_ReturnsRuleSet()
    {
        var json = """
{
  "layers": [
    { "name": "Domain", "patterns": ["MyApp.Domain.*"] },
    { "name": "Application", "patterns": [] }
  ],
  "violations": [
    { "from": "Application", "to": "Infrastructure" }
  ]
}
""";
        var filePath = WriteTempFile(json);

        try
        {
            var ruleSet = RuleSetLoader.LoadFromFile(filePath);

            Assert.Equal(2, ruleSet.Layers.Count);
            Assert.Equal("Domain", ruleSet.Layers[0].Name);
            Assert.Equal(["MyApp.Domain.*"], ruleSet.Layers[0].Patterns);
            Assert.Equal("Application", ruleSet.Layers[1].Name);
            Assert.Empty(ruleSet.Layers[1].Patterns);

            Assert.Single(ruleSet.Violations);
            Assert.Equal("Application", ruleSet.Violations[0].From);
            Assert.Equal("Infrastructure", ruleSet.Violations[0].To);
        }
        finally
        {
            DeleteTempFile(filePath);
        }
    }

    /// <summary>
    /// ファイルが存在しない場合に例外が発生することを確認する。
    /// </summary>
    [Fact]
    public void LoadFromFile_WithMissingFile_Throws()
    {
        var missingPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.json");

        Assert.Throws<ArgumentException>(() => RuleSetLoader.LoadFromFile(missingPath));
    }

    /// <summary>
    /// JSON が不正な場合に例外が発生することを確認する。
    /// </summary>
    [Fact]
    public void LoadFromFile_WithInvalidJson_Throws()
    {
        var filePath = WriteTempFile("{");

        try
        {
            Assert.Throws<ArgumentException>(() => RuleSetLoader.LoadFromFile(filePath));
        }
        finally
        {
            DeleteTempFile(filePath);
        }
    }

    /// <summary>
    /// レイヤー名が空の場合に例外が発生することを確認する。
    /// </summary>
    [Fact]
    public void LoadFromFile_WithMissingLayerName_Throws()
    {
        var json = """
{
  "layers": [
    { "name": "", "patterns": ["MyApp.Domain.*"] }
  ],
  "violations": [
    { "from": "Application", "to": "Infrastructure" }
  ]
}
""";
        var filePath = WriteTempFile(json);

        try
        {
            Assert.Throws<ArgumentException>(() => RuleSetLoader.LoadFromFile(filePath));
        }
        finally
        {
            DeleteTempFile(filePath);
        }
    }

    /// <summary>
    /// violation の必須項目が欠落している場合に例外が発生することを確認する。
    /// </summary>
    [Fact]
    public void LoadFromFile_WithMissingViolationFields_Throws()
    {
        var json = """
{
  "layers": [
    { "name": "Application", "patterns": ["MyApp.App.*"] }
  ],
  "violations": [
    { "from": "Application", "to": "" }
  ]
}
""";
        var filePath = WriteTempFile(json);

        try
        {
            Assert.Throws<ArgumentException>(() => RuleSetLoader.LoadFromFile(filePath));
        }
        finally
        {
            DeleteTempFile(filePath);
        }
    }

    /// <summary>
    /// violations が空配列の場合に空のリストとして扱われることを確認する。
    /// </summary>
    [Fact]
    public void LoadFromFile_WithEmptyViolations_ReturnsEmptyViolationList()
    {
        var json = """
{
  "layers": [
    { "name": "Application", "patterns": ["MyApp.App.*"] }
  ],
  "violations": []
}
""";
        var filePath = WriteTempFile(json);

        try
        {
            var ruleSet = RuleSetLoader.LoadFromFile(filePath);

            Assert.Single(ruleSet.Layers);
            Assert.Empty(ruleSet.Violations);
        }
        finally
        {
            DeleteTempFile(filePath);
        }
    }

    /// <summary>
    /// テスト用の一時 JSON ファイルを作成する。
    /// </summary>
    private static string WriteTempFile(string content)
    {
        var directory = Path.Combine(Path.GetTempPath(), "CodeDepsJiroTests");
        Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, $"{Guid.NewGuid():N}.json");
        File.WriteAllText(filePath, content);
        return filePath;
    }

    /// <summary>
    /// テスト用の一時ファイルを削除する。
    /// </summary>
    private static void DeleteTempFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
