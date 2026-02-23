using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DepGraph.ProjectLoader;

/// <summary>
/// .csproj またはフォルダから .cs ファイル一覧を解決するクラス
/// </summary>
public sealed class ProjectLoader : IProjectLoader
{
    /// <summary>
    /// .csproj またはディレクトリから C# ソースファイル一覧を解決する。
    /// </summary>
    /// <param name="inputPath">.csproj またはディレクトリのパス。</param>
    /// <param name="excludePattern">除外パターン。</param>
    /// <returns>解決された C# ソースファイル一覧。</returns>
    /// <exception cref="ArgumentException">入力が不正な場合。</exception>
    public IReadOnlyList<string> LoadSourceFiles(string inputPath, string? excludePattern)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            throw new ArgumentException("Input path is required.");
        }

        var fullPath = Path.GetFullPath(inputPath);
        if (File.Exists(fullPath))
        {
            if (!string.Equals(Path.GetExtension(fullPath), ".csproj", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Input file must be a .csproj.");
            }

            return LoadFromProject(fullPath, excludePattern, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }

        if (Directory.Exists(fullPath))
        {
            return EnumerateCsFiles(fullPath, excludePattern);
        }

        throw new ArgumentException($"Input path not found: {inputPath}");
    }

    /// <summary>
    /// .csproj を解析してソースファイルを解決する。
    /// </summary>
    /// <param name="projectPath">.csproj のパス。</param>
    /// <param name="excludePattern">除外パターン。</param>
    /// <param name="visited">解析済みプロジェクト。</param>
    /// <returns>ソースファイル一覧。</returns>
    private static IReadOnlyList<string> LoadFromProject(
        string projectPath,
        string? excludePattern,
        HashSet<string> visited)
    {
        if (!visited.Add(Path.GetFullPath(projectPath)))
        {
            return Array.Empty<string>();
        }

        var projectDir = Path.GetDirectoryName(projectPath) ?? throw new ArgumentException("Invalid project path.");
        var defaultFiles = EnumerateCsFiles(projectDir, excludePattern);

        var document = XDocument.Load(projectPath);
        var root = document.Root ?? throw new ArgumentException("Invalid project file.");

        var includePaths = ResolveProjectIncludes(root, projectDir);
        var removePaths = ResolveProjectRemoves(root, projectDir);
        var projectRefs = ResolveProjectReferences(root, projectDir);

        var files = new HashSet<string>(defaultFiles, StringComparer.OrdinalIgnoreCase);
        foreach (var include in includePaths)
        {
            if (File.Exists(include))
            {
                files.Add(include);
            }
            else if (Directory.Exists(include))
            {
                foreach (var file in EnumerateCsFiles(include, excludePattern))
                {
                    files.Add(file);
                }
            }
        }

        foreach (var remove in removePaths)
        {
            files.RemoveWhere(path => MatchesPath(remove, path));
        }

        foreach (var reference in projectRefs)
        {
            foreach (var file in LoadFromProject(reference, excludePattern, visited))
            {
                files.Add(file);
            }
        }

        return files.ToList();
    }

    /// <summary>
    /// Compile Include を解決する。
    /// </summary>
    /// <param name="root">プロジェクト XML。</param>
    /// <param name="projectDir">プロジェクトのディレクトリ。</param>
    /// <returns>インクルード対象パス一覧。</returns>
    private static IReadOnlyList<string> ResolveProjectIncludes(XElement root, string projectDir)
    {
        return root.Descendants()
            .Where(node => node.Name.LocalName == "Compile" && node.Attribute("Include") != null)
            .Select(node => ResolvePath(projectDir, node.Attribute("Include")!.Value))
            .ToList();
    }

    /// <summary>
    /// Compile Remove を解決する。
    /// </summary>
    /// <param name="root">プロジェクト XML。</param>
    /// <param name="projectDir">プロジェクトのディレクトリ。</param>
    /// <returns>除外対象パス一覧。</returns>
    private static IReadOnlyList<string> ResolveProjectRemoves(XElement root, string projectDir)
    {
        return root.Descendants()
            .Where(node => node.Name.LocalName == "Compile" && node.Attribute("Remove") != null)
            .Select(node => ResolvePath(projectDir, node.Attribute("Remove")!.Value))
            .ToList();
    }

    /// <summary>
    /// ProjectReference を解決する。
    /// </summary>
    /// <param name="root">プロジェクト XML。</param>
    /// <param name="projectDir">プロジェクトのディレクトリ。</param>
    /// <returns>参照プロジェクトの一覧。</returns>
    private static IReadOnlyList<string> ResolveProjectReferences(XElement root, string projectDir)
    {
        return root.Descendants()
            .Where(node => node.Name.LocalName == "ProjectReference" && node.Attribute("Include") != null)
            .Select(node => ResolvePath(projectDir, node.Attribute("Include")!.Value))
            .Where(File.Exists)
            .ToList();
    }

    /// <summary>
    /// 相対パスをプロジェクトディレクトリ基準で解決する。
    /// </summary>
    /// <param name="projectDir">プロジェクトのディレクトリ。</param>
    /// <param name="path">パス。</param>
    /// <returns>絶対パス。</returns>
    private static string ResolvePath(string projectDir, string path)
    {
        return Path.GetFullPath(Path.Combine(projectDir, path));
    }

    /// <summary>
    /// 指定したルート配下の .cs ファイルを列挙し、除外条件を適用する。
    /// </summary>
    /// <param name="root">探索ルート。</param>
    /// <param name="excludePattern">除外パターン。</param>
    /// <returns>列挙されたファイル一覧。</returns>
    private static IReadOnlyList<string> EnumerateCsFiles(string root, string? excludePattern)
    {
        var files = Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(path => !IsInIgnoredDirectory(path))
            .Where(path => !IsExcluded(path, root, excludePattern))
            .ToList();

        return files;
    }

    /// <summary>
    /// bin/ や obj/ などの生成物ディレクトリを除外する。
    /// </summary>
    /// <param name="path">ファイルパス。</param>
    /// <returns>除外対象の場合 true。</returns>
    private static bool IsInIgnoredDirectory(string path)
    {
        var normalized = path.Replace('\\', '/');
        return normalized.Contains("/bin/") || normalized.Contains("/obj/");
    }

    /// <summary>
    /// 指定された除外パターンに一致するパスを除外する。
    /// </summary>
    /// <param name="path">ファイルパス。</param>
    /// <param name="root">探索ルート。</param>
    /// <param name="excludePattern">除外パターン。</param>
    /// <returns>除外対象の場合 true。</returns>
    private static bool IsExcluded(string path, string root, string? excludePattern)
    {
        if (string.IsNullOrWhiteSpace(excludePattern))
        {
            return false;
        }

        var relative = Path.GetRelativePath(root, path).Replace('\\', '/');
        return WildcardMatch(excludePattern, relative) || WildcardMatch(excludePattern, Path.GetFileName(path));
    }

    /// <summary>
    /// 除外指定のパスと実ファイルの一致を判定する。
    /// </summary>
    /// <param name="patternPath">除外指定のパス。</param>
    /// <param name="actualPath">実ファイルのパス。</param>
    /// <returns>一致する場合 true。</returns>
    private static bool MatchesPath(string patternPath, string actualPath)
    {
        var normalizedPattern = patternPath.Replace('\\', '/');
        var normalizedActual = actualPath.Replace('\\', '/');

        if (normalizedPattern.Contains('*') || normalizedPattern.Contains('?'))
        {
            return WildcardMatch(normalizedPattern, normalizedActual);
        }

        return string.Equals(normalizedPattern, normalizedActual, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// ワイルドカード（* / ?）を含む簡易マッチを行う。
    /// </summary>
    /// <param name="pattern">パターン。</param>
    /// <param name="text">判定対象。</param>
    /// <returns>一致する場合 true。</returns>
    private static bool WildcardMatch(string pattern, string text)
    {
        var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$";
        return System.Text.RegularExpressions.Regex.IsMatch(text, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}
