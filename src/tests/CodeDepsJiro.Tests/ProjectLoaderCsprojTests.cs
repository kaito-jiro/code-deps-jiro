using ProjectLoaderType = CodeDepsJiro.ProjectLoader.ProjectLoader;

namespace CodeDepsJiro.Tests;

public sealed class ProjectLoaderCsprojTests
{
    /// <summary>
    /// Compile Include/Remove が適用されることを確認する。
    /// </summary>
    [Fact]
    public void LoadSourceFiles_WithCompileIncludeRemove_AppliesFilters()
    {
        var root = CreateTempDirectory();
        try
        {
            var srcDir = Path.Combine(root, "src");
            Directory.CreateDirectory(srcDir);

            var includedA = WriteFile(Path.Combine(srcDir, "A.cs"), "namespace Sample; public class A {}");
            var includedB = WriteFile(Path.Combine(srcDir, "B.cs"), "namespace Sample; public class B {}");
            var removed = WriteFile(Path.Combine(srcDir, "Generated.cs"), "namespace Sample; public class Generated {}");

            var projectPath = Path.Combine(root, "Sample.csproj");
            WriteFile(projectPath, """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="src/*.cs" />
    <Compile Remove="src/Generated.cs" />
  </ItemGroup>
</Project>
""");

            var loader = new ProjectLoaderType();
            var files = loader.LoadSourceFiles(projectPath, null);
            var set = new HashSet<string>(files, StringComparer.OrdinalIgnoreCase);

            Assert.Contains(includedA, set);
            Assert.Contains(includedB, set);
            Assert.DoesNotContain(removed, set);
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    /// <summary>
    /// ProjectReference 参照先の .csproj が解決されることを確認する。
    /// </summary>
    [Fact]
    public void LoadSourceFiles_WithProjectReference_IncludesReferencedProjectFiles()
    {
        var root = CreateTempDirectory();
        try
        {
            var mainDir = Path.Combine(root, "Main");
            var subDir = Path.Combine(root, "Sub");
            Directory.CreateDirectory(mainDir);
            Directory.CreateDirectory(subDir);

            var mainFile = WriteFile(Path.Combine(mainDir, "Main.cs"), "namespace Main; public class MainClass {}");
            var subFile = WriteFile(Path.Combine(subDir, "Sub.cs"), "namespace Sub; public class SubClass {}");

            var subProjectPath = Path.Combine(subDir, "Sub.csproj");
            WriteFile(subProjectPath, """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
</Project>
""");

            var mainProjectPath = Path.Combine(mainDir, "Main.csproj");
            WriteFile(mainProjectPath, """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\\Sub\\Sub.csproj" />
  </ItemGroup>
</Project>
""");

            var loader = new ProjectLoaderType();
            var files = loader.LoadSourceFiles(mainProjectPath, null);
            var set = new HashSet<string>(files, StringComparer.OrdinalIgnoreCase);

            Assert.Contains(mainFile, set);
            Assert.Contains(subFile, set);
        }
        finally
        {
            DeleteDirectory(root);
        }
    }

    /// <summary>
    /// テスト用の一時ディレクトリを作成する。
    /// </summary>
    private static string CreateTempDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "CodeDepsJiroTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    /// <summary>
    /// テスト用のファイルを書き込む。
    /// </summary>
    private static string WriteFile(string path, string content)
    {
        File.WriteAllText(path, content);
        return path;
    }

    /// <summary>
    /// テスト用ディレクトリを削除する。
    /// </summary>
    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
}
