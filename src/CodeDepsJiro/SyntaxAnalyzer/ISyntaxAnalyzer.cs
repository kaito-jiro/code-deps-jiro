using System.Collections.Generic;

namespace CodeDepsJiro.SyntaxAnalyzer;

public interface ISyntaxAnalyzer
{
    SyntaxAnalysisResult Analyze(IReadOnlyList<string> sourceFiles);
}
