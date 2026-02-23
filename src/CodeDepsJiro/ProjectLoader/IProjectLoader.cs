using System.Collections.Generic;

namespace CodeDepsJiro.ProjectLoader;

public interface IProjectLoader
{
    IReadOnlyList<string> LoadSourceFiles(string inputPath, string? excludePattern);
}
