using System.Collections.Generic;

namespace DepGraph.SyntaxAnalyzer;

public interface ISyntaxAnalyzer
{
    SyntaxAnalysisResult Analyze(IReadOnlyList<string> sourceFiles);
}
