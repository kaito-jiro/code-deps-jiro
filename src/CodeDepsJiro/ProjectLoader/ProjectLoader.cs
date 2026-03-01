using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeDepsJiro.ProjectLoader;

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

        var includePaths = ResolveProjectIncludes(root);
        var removePaths = ResolveProjectRemoves(root);
        var projectRefs = ResolveProjectReferences(root, projectDir);

        var files = new HashSet<string>(defaultFiles, StringComparer.OrdinalIgnoreCase);
        foreach (var include in includePaths)
        {
            foreach (var file in ExpandInclude(projectDir, include, excludePattern))
            {
                files.Add(file);
            }
        }

        foreach (var remove in removePaths)
        {
            files.RemoveWhere(path => MatchesRemovePattern(projectDir, remove, path));
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
    /// <returns>インクルード対象パス一覧。</returns>
    private static IReadOnlyList<string> ResolveProjectIncludes(XElement root)
    {
        return root.Descendants()
            .Where(node => node.Name.LocalName == "Compile" && node.Attribute("Include") != null)
            .SelectMany(node => SplitItemValues(node.Attribute("Include")!.Value))
            .ToList();
    }

    /// <summary>
    /// Compile Remove を解決する。
    /// </summary>
    /// <param name="root">プロジェクト XML。</param>
    /// <returns>除外対象パス一覧。</returns>
    private static IReadOnlyList<string> ResolveProjectRemoves(XElement root)
    {
        return root.Descendants()
            .Where(node => node.Name.LocalName == "Compile" && node.Attribute("Remove") != null)
            .SelectMany(node => SplitItemValues(node.Attribute("Remove")!.Value))
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
        var normalized = path.Replace('\\', Path.DirectorySeparatorChar);
        return Path.GetFullPath(Path.Combine(projectDir, normalized));
    }

    /// <summary>
    /// Compile Include の値を展開してファイル一覧を取得する。
    /// </summary>
    /// <param name="projectDir">プロジェクトのディレクトリ。</param>
    /// <param name="include">Include パターン。</param>
    /// <param name="excludePattern">除外パターン。</param>
    /// <returns>対象ファイル一覧。</returns>
    private static IEnumerable<string> ExpandInclude(string projectDir, string include, string? excludePattern)
    {
        if (string.IsNullOrWhiteSpace(include))
        {
            yield break;
        }

        if (ContainsWildcard(include))
        {
            foreach (var file in EnumerateMatchingFiles(projectDir, include, excludePattern))
            {
                yield return file;
            }

            yield break;
        }

        var path = ResolvePath(projectDir, include);
        if (File.Exists(path))
        {
            yield return path;
            yield break;
        }

        if (Directory.Exists(path))
        {
            foreach (var file in EnumerateCsFiles(path, excludePattern))
            {
                yield return file;
            }
        }
    }

    /// <summary>
    /// ワイルドカードを含む Include から対象ファイルを列挙する。
    /// </summary>
    /// <param name="projectDir">プロジェクトのディレクトリ。</param>
    /// <param name="pattern">検索パターン。</param>
    /// <param name="excludePattern">除外パターン。</param>
    /// <returns>一致したファイル一覧。</returns>
    private static IReadOnlyList<string> EnumerateMatchingFiles(string projectDir, string pattern, string? excludePattern)
    {
        var files = Directory.EnumerateFiles(projectDir, "*.cs", SearchOption.AllDirectories)
            .Where(path => !IsInIgnoredDirectory(path))
            .Where(path => !IsExcluded(path, projectDir, excludePattern))
            .Where(path => MatchesIncludePattern(projectDir, pattern, path))
            .ToList();

        return files;
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
    private static bool MatchesRemovePattern(string projectDir, string patternPath, string actualPath)
    {
        var normalizedActual = NormalizePath(actualPath);
        var normalizedPattern = NormalizePath(patternPath);

        if (Path.IsPathRooted(patternPath))
        {
            var fullPattern = NormalizePath(Path.GetFullPath(patternPath));
            return ContainsWildcard(fullPattern)
                ? WildcardMatch(fullPattern, normalizedActual)
                : string.Equals(fullPattern, normalizedActual, StringComparison.OrdinalIgnoreCase);
        }

        var relative = NormalizePath(Path.GetRelativePath(projectDir, actualPath));
        if (ContainsWildcard(normalizedPattern))
        {
            return WildcardMatch(normalizedPattern, relative) ||
                WildcardMatch(normalizedPattern, Path.GetFileName(actualPath));
        }

        if (string.Equals(normalizedPattern, relative, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var resolved = NormalizePath(ResolvePath(projectDir, patternPath));
        return string.Equals(resolved, normalizedActual, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Include パターンと実ファイルの一致を判定する。
    /// </summary>
    /// <param name="projectDir">プロジェクトのディレクトリ。</param>
    /// <param name="pattern">Include パターン。</param>
    /// <param name="actualPath">実ファイルのパス。</param>
    /// <returns>一致する場合 true。</returns>
    private static bool MatchesIncludePattern(string projectDir, string pattern, string actualPath)
    {
        var normalizedActual = NormalizePath(actualPath);
        var normalizedPattern = NormalizePath(pattern);

        if (Path.IsPathRooted(pattern))
        {
            var fullPattern = NormalizePath(Path.GetFullPath(pattern));
            return WildcardMatch(fullPattern, normalizedActual);
        }

        var relative = NormalizePath(Path.GetRelativePath(projectDir, actualPath));
        return WildcardMatch(normalizedPattern, relative);
    }

    /// <summary>
    /// ワイルドカードを含むか判定する。
    /// </summary>
    /// <param name="pattern">パターン。</param>
    /// <returns>含む場合 true。</returns>
    private static bool ContainsWildcard(string pattern)
    {
        return pattern.Contains('*') || pattern.Contains('?');
    }

    /// <summary>
    /// パスを正規化する。
    /// </summary>
    /// <param name="path">対象パス。</param>
    /// <returns>正規化済みパス。</returns>
    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }

    /// <summary>
    /// セミコロン区切りの項目を分割する。
    /// </summary>
    /// <param name="value">値。</param>
    /// <returns>分割結果。</returns>
    private static IEnumerable<string> SplitItemValues(string value)
    {
        return value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
