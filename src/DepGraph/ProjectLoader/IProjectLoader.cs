using System.Collections.Generic;

namespace DepGraph.ProjectLoader;

public interface IProjectLoader
{
    IReadOnlyList<string> LoadSourceFiles(string inputPath, string? excludePattern);
}
